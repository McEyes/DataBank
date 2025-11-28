
using DataQualityAssessment.Application.Assessments.Dtos;
using DataQualityAssessment.Application.Common;
using DataQualityAssessment.Application.Rules.Services;
using DataQualityAssessment.Core.Entities;
using DataQualityAssessment.Core.Entities.DataAsset;
using DataQualityAssessment.Core.Enums;
using DataQualityAssessment.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using DataQualityAssessment.Core.Extensions;
using Microsoft.EntityFrameworkCore.Internal;
using DataQualityAssessment.Core.DbContextLocators;
using DataQualityAssessment.Application.DataAssets.Services;
using DataQualityAssessment.Application.DataAssets.Dtos;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataQualityAssessment.Application.Assessments.Services
{
    public class AssessmentService : IAssessmentService, ITransient
    {

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ITableRuleService _tableRuleService;
        public AssessmentService(
            IServiceScopeFactory scopeFactory,
            ITableRuleService tableRuleService)
        {
            _scopeFactory = scopeFactory;
            _tableRuleService = tableRuleService;
        }

        public async Task<IEnumerable<AssessmentResultDto>> GetWaitingListAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var excludeErrorTypes = new TableErrorType[] {
                    TableErrorType.UnknownTable,
                    TableErrorType.UnknownColumn,
                    TableErrorType.ConnectionTimeout,
                    TableErrorType.UnableToConnectServer
                };
                var serviceProvider = scope.ServiceProvider;
                var assessmentResultRepository = Db.GetRepository<AssessmentResultEntity>(serviceProvider);
                var dataAssetService = App.GetRequiredService<IDataAssetService>(serviceProvider);
                //var validTableIds = dataAssetService.GetValidTableIds();

                var list = await assessmentResultRepository.Where(t => t.Status == ProgressStatus.Waiting
                || (t.Status == ProgressStatus.InEvaluation && t.RepeatCount < 50))
                    .Where(t => !excludeErrorTypes.Any(a => a == t.ErrorType)).ToListAsync();
                //.Where(t => validTableIds.Any(a => a == t.Id)).ToListAsync();

                return list.Adapt<List<AssessmentResultDto>>();
            }
        }

        public async Task SaveReportsAsync(IEnumerable<AssessmentReportEntity> entities)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var assessmentReportRepository = Db.GetRepository<AssessmentReportEntity>(serviceProvider);
                var assessmentReportHistoryRepository = Db.GetRepository<AssessmentReportHistoryEntity>(serviceProvider);

                if (entities.Any())
                {
                    var deleteEntities = assessmentReportRepository.Where(t => entities.Select(a => a.TableId).Any(a => a == t.TableId));
                    await assessmentReportRepository.DeleteAsync(deleteEntities);
                    await assessmentReportRepository.SaveNowAsync();

                    var now = DateTime.Now;
                    foreach (var entity in entities)
                    {
                        entity.CreatedTime = now;

                        await assessmentReportRepository.InsertAsync(entity);
                        await assessmentReportHistoryRepository.InsertAsync(entity.Adapt<AssessmentReportHistoryEntity>());
                    }

                    await assessmentReportRepository.SaveNowAsync();
                    await assessmentReportHistoryRepository.SaveNowAsync();
                }
            }
        }

        public async Task<List<AssessmentReportItemDto>> GetReportsAsync(string tableId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var assessmentReportRepository = Db.GetRepository<AssessmentReportEntity>(serviceProvider);
                var assessmentResultRepository = Db.GetRepository<AssessmentResultEntity>(serviceProvider);
                var ruleRepository = Db.GetRepository<RuleEntity>(serviceProvider);
                var resultEntity = await assessmentResultRepository.FirstOrDefaultAsync(t => t.Id == tableId);

                if (resultEntity == null || resultEntity.Status != ProgressStatus.Finished)
                {
                    return new List<AssessmentReportItemDto>();
                }

                var queryReport = assessmentReportRepository.AsQueryable();
                var queryRule = ruleRepository.AsQueryable();
                var regex = new Regex("column names=(.+\r?)");

                var result = await queryReport.Where(t => t.TableId == tableId).Join(queryRule, a => a.RuleNo, a => a.Id, (a, b) => new AssessmentReportItemDto
                {
                    Weight = a.Weight,
                    MonitoringLevel = b.MonitoringLevel,
                    Description = b.Description,
                    RuleName = b.Name,
                    RuleNo = a.RuleNo,
                    Score = Math.Round(a.Score * 100, 2),
                    TableId = a.TableId,
                    ValidateType = a.ValidateType,
                    Fields = regex.Match(a.Description).Value.Trim().Replace("column names=","").Replace("all table","")
                }
               ).ToListAsync();

                return result?.OrderBy(t => t.ValidateType)?.ToList();
            }
        }

        public async Task AddToEvaluateAsync(string tableId)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(tableId, nameof(tableId));

            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var results = await _tableRuleService.GetTableRulesAsync(tableId);
                if (results.Count == 0)
                {
                    Oops.Oh($"Please select the table mapping rules and set the weight value for each rule");
                }

                var now = DateTime.Now;
                var assessmentResultRepository = Db.GetRepository<AssessmentResultEntity>(serviceProvider);
                var entity = await assessmentResultRepository.FirstOrDefaultAsync(t => t.Id == tableId);

                if (entity != null)
                {
                    entity.UpdatedTime = null;
                    entity.Status = ProgressStatus.Waiting;
                    //entity.Score = 0;
                    entity.EndTime = null;
                    entity.ElapsedMilliseconds = 0;
                    entity.RepeatCount = 0;
                    entity.LastException = null;

                    await assessmentResultRepository.UpdateNowAsync(entity);
                }
                else
                {
                    entity = new AssessmentResultEntity();

                    entity.Id = tableId;
                    entity.CreatedTime = now;
                    entity.UpdatedTime = null;
                    entity.Status = ProgressStatus.Waiting;
                    entity.Score = 0;
                    entity.EndTime = null;
                    entity.ElapsedMilliseconds = 0;
                    entity.RepeatCount = 0;

                    await assessmentResultRepository.InsertNowAsync(entity);
                }
            }

        }

        public async Task BeginEvaluateAsync(string tableId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var now = DateTime.Now;
                var serviceProvider = scope.ServiceProvider;
                var assessmentResultRepository = Db.GetRepository<AssessmentResultEntity>(serviceProvider);
                var result = await assessmentResultRepository.FirstOrDefaultAsync(t => t.Id == tableId);

                if (result == null)
                {
                    result = new AssessmentResultEntity();
                    result.Id = tableId;
                    result.Status = ProgressStatus.InEvaluation;
                    result.ExecutionTime = now;
                    result.CreatedTime = now;

                    await assessmentResultRepository.InsertNowAsync(result);
                }
                else
                {
                    result.ExecutionTime = now;
                    result.UpdatedTime = now;
                    result.Status = ProgressStatus.InEvaluation;
                    //result.Score = null;
                    result.ElapsedMilliseconds = null;
                    result.EndTime = null;
                    result.RepeatCount = result.RepeatCount ?? 0 + 1;

                    await assessmentResultRepository.UpdateNowAsync(result);
                }
            }
        }

        public async Task EndEvaluateAsync(string tableId, double newScore)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var now = DateTime.Now;
                var serviceProvider = scope.ServiceProvider;
                var assessmentResultRepository = Db.GetRepository<AssessmentResultEntity>(serviceProvider);
                var tableRepository = Db.GetRepository<TableEntity, DataAssetDbContextLocator>(serviceProvider);
                var result = await assessmentResultRepository.FirstOrDefaultAsync(t => t.Id == tableId);
                double? lastScore = null;
                if (result != null)
                {
                    lastScore = result.Score;
                    result.UpdatedTime = now;
                    result.Status = ProgressStatus.Finished;
                    result.LastScore = lastScore;
                    result.Score = newScore;
                    result.EndTime = now;
                    result.ElapsedMilliseconds = (result.EndTime - result.ExecutionTime).Value.TotalMilliseconds;

                    await assessmentResultRepository.UpdateNowAsync(result);
                }

                var entity = new TableEntity
                {
                    Id = tableId,
                    quality_score = (int)(newScore * 100),
                    last_score = lastScore == null ? null : (int)(lastScore * 100)
                };

                await tableRepository.UpdateIncludeNowAsync(entity, "quality_score,last_score".Split(","));
            }
        }

        public async Task SaveExceptionAsync(string tableId, string message)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var now = DateTime.Now;
                var serviceProvider = scope.ServiceProvider;
                var assessmentResultRepository = Db.GetRepository<AssessmentResultEntity>(serviceProvider);
                var result = await assessmentResultRepository.FirstOrDefaultAsync(t => t.Id == tableId);
                if (result != null)
                {
                    result.UpdatedTime = now;
                    result.LastException = message;
                    result.RepeatCount += result.RepeatCount == null ? 1 : result.RepeatCount + 1;

                    if (message.Contains(TableErrorType.UnknownTable.GetDescription(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.ErrorType = TableErrorType.UnknownTable;
                    }
                    else if (message.Contains(TableErrorType.UnknownColumn.GetDescription(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.ErrorType = TableErrorType.UnknownColumn;
                    }
                    else if (message.Contains(TableErrorType.SqlBlockRule.GetDescription(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.ErrorType = TableErrorType.SqlBlockRule;
                    }
                    else if (message.Contains(TableErrorType.ConnectionTimeout.GetDescription(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.ErrorType = TableErrorType.ConnectionTimeout;
                    }
                    else if (message.Contains(TableErrorType.UnableToConnectServer.GetDescription(), StringComparison.OrdinalIgnoreCase))
                    {
                        result.ErrorType = TableErrorType.UnableToConnectServer;
                    }
                    else
                    {
                        result.ErrorType = TableErrorType.UnKnown;
                    }

                    await assessmentResultRepository.UpdateNowAsync(result);
                }
            }
        }

        public async Task<PagedResultDto<TablePageItemDto>> GetPagingListAsync(TableSearchDto input)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                if (input.PageIndex <= 0) input.PageIndex = 1;
                if (input.PageSize <= 0) input.PageSize = 10;

                var pagedResult = new PagedResultDto<TablePageItemDto>();
                var serviceProvider = scope.ServiceProvider;

                var tableRepository = Db.GetRepository<TableEntity, DataAssetDbContextLocator>(serviceProvider);
                var sourceRepository = Db.GetRepository<SourceEntity, DataAssetDbContextLocator>(serviceProvider);
                var authorizeownerRepository = Db.GetRepository<AuthorizeOwnerEntity, DataAssetDbContextLocator>(serviceProvider);

                var assessmentResultRepository = Db.GetRepository<AssessmentResultEntity>(serviceProvider);
                var tableRuleRepository = Db.GetRepository<TableRulesEntity>(serviceProvider);

                var dataAssetService = App.GetRequiredService<IDataAssetService>(serviceProvider);
                var validTableIds = dataAssetService.GetValidTableIds();

                var scoreFrom = (input.ScoreFrom ?? 0) / 100D;
                var scoreTo = (input.ScoreTo ?? 0) / 100D;

                var authorizeownerQuery = authorizeownerRepository.AsQueryable();
                var authorizeownerList = new Dictionary<string, List<string>>();
                var includeTableIds = assessmentResultRepository
                    .Where(input.ScoreFrom != null, t => t.Score >= scoreFrom)
                    .Where(input.ScoreTo != null, t => t.Score <= scoreTo)
                    .Select(t => t.Id).Distinct().ToList();

                var queryTable = tableRepository
                    .Where(validTableIds != null, a => validTableIds.Any(t => t == a.Id))
                    .Where(input.ScoreFrom != null || input.ScoreTo != null, a => includeTableIds.Any(t => t == a.Id))
                    .Where(!string.IsNullOrWhiteSpace(input.TableId), a => a.Id == input.TableId)
                    .Where(!string.IsNullOrWhiteSpace(input.TableName), a => a.table_name.Contains(input.TableName));

                // owner
                if (!string.IsNullOrWhiteSpace(input.Owner))
                {
                    authorizeownerList = authorizeownerQuery
                        .Where(t => t.owner_id.ToLower() == input.Owner.ToLower() && t.object_type.ToLower() == "table")
                        .GroupBy(t => t.object_id).ToDictionary(t => t.Key, t => t.Select(a => a.owner_name).ToList());
                    if (authorizeownerList.Count > 0)
                    {
                        var tableids = authorizeownerList.Keys.Select(t => t);
                        queryTable = queryTable.Where(t => tableids.Any(a => a == t.Id));
                    }
                    else
                    {
                        return pagedResult;
                    }
                }

                // dept
                if (!string.IsNullOrWhiteSpace(input.Dept))
                {
                    authorizeownerList = authorizeownerQuery
                        .Where(t => t.owner_dept.ToLower() == input.Dept.ToLower() && t.object_type.ToLower() == "table")
                        .GroupBy(t => t.object_id).ToDictionary(t => t.Key, t => t.Select(a => a.owner_name).ToList());
                    if (authorizeownerList.Count > 0)
                    {
                        var tableids = authorizeownerList.Keys.Select(t => t);
                        queryTable = queryTable.Where(t => tableids.Any(a => a == t.Id));
                    }
                    else
                    {
                        return pagedResult;
                    }
                }

                var totalCount = await queryTable.CountAsync();
                var pagedList = queryTable.OrderByDescending(t => t.Id).ToPagedList(input.PageIndex, input.PageSize, () =>
                {
                    return totalCount;
                });


                pagedResult.TotalCount = pagedList.TotalCount;
                pagedResult.Data = pagedList.Items.Adapt<List<TablePageItemDto>>();

                var sourceIds = pagedResult.Data.Select(t => t.SourceId).Distinct();
                var tableIds = pagedResult.Data.Select(t => t.TableId).Distinct();
                var sourceList = sourceRepository.Where(t => sourceIds.Any(a => a == t.Id)).ToList();
                var assesResults = assessmentResultRepository.Where(t => tableIds.Any(a => a == t.Id)).ToList();
                var tableRules = tableRuleRepository.Where(t => tableIds.Any(a => a == t.TableId));
                var authorizeowners = authorizeownerRepository.Where(t => tableIds.Any(a => a == t.object_id) && t.object_type.ToLower() == "table").ToList();

                foreach (var item in pagedResult.Data)
                {
                    item.DataOwner = "System";

                    var sourceItem = sourceList.FirstOrDefault(t => t.Id == item.SourceId);
                    var resultItem = assesResults.FirstOrDefault(t => t.Id == item.TableId);
                    var dataowners = authorizeowners.Where(t => t.object_id == item.TableId).ToList();

                    if (sourceItem != null)
                    {
                        var dbShema = JsonConvert.DeserializeObject<DbSchemaModel>(sourceItem.db_schema);
                        item.SourceName = sourceItem.source_name;
                        item.DbName = dbShema?.dbName;
                    }

                    if (resultItem != null)
                    {
                        item.Status = resultItem.Status;
                        item.ExecutionTime = resultItem.ExecutionTime;
                        item.Score = resultItem.Score == null ? null : (int)(resultItem.Score * 100);
                        item.LastScore = resultItem.LastScore == null ? null : (int)(resultItem.LastScore * 100);
                    }
                    else
                    {
                        item.Status = null;
                    }

                    if (tableRules != null)
                        item.RuleCount = tableRules.Count(t => t.TableId == item.TableId);

                    if (dataowners != null)
                    {
                        item.OwnerDept = string.Join(",", dataowners.Select(t => t.owner_dept));
                        item.DataOwner = string.Join(",", dataowners.Select(t => t.owner_name));
                    }
                }

                return pagedResult;
            }
        }

        public async Task InitDataAsync(string secret)
        {
            if (secret != "123") Oops.Oh($"secret error!");

            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var tableRuleRepository = Db.GetRepository<TableRulesEntity>(serviceProvider);

                var query = tableRuleRepository.AsQueryable();
                var tableIds = query.GroupBy(t => t.TableId).Select(t => t.Key).ToList();
                foreach (var tableId in tableIds)
                {
                    await AddToEvaluateAsync(tableId);
                    Thread.Sleep(10);
                }
            }
        }
    }
}
