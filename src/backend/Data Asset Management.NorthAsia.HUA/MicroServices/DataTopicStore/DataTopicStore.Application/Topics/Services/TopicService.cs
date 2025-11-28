using System.Text;
using Dapper;
using DataTopicStore.Application.Categories.Dtos;
using DataTopicStore.Application.Categories.Services;
using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Helpers;
using DataTopicStore.Application.Logs.Services;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Core.Caches;
using DataTopicStore.Core.Datalineage.Openlineage;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Enums;
using DataTopicStore.Core.Extensions;
using DataTopicStore.Core.Models;
using DataTopicStore.Core.Repositories;
using Mapster;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using System.Diagnostics;
using Magicodes.ExporterAndImporter.Excel.AspNetCore;
using System.Data;
using Magicodes.ExporterAndImporter.Core.Extension;
using System.Collections.Generic;
using Dm;
using MySqlX.XDevAPI.Relational;
using Microsoft.AspNetCore.Http.HttpResults;
using Mysqlx.Crud;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using DataTopicStore.Application.ApproveFlow;
using Microsoft.CodeAnalysis;
using System;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Furion.UnifyResult;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace DataTopicStore.Application.Topics.Services
{
    [AppAuthorize]
    public class TopicService : ApplicationService, ITopicService
    {
        private readonly IDapperRepository dapperRepository;
        private readonly Repository<TopicEntity> topicRepository;
        private readonly Repository<TopicLikesEntity> topicLikeRepository;
        private readonly Repository<TopicFavoritesEntity> topicFavoriteRepository;
        private readonly Repository<TopicGoalEntity> topicTopicGoalRepository;
        private readonly ICategoryService categoryService;
        private readonly FlowApplyProxyService flowApplyProxyService;
        private readonly ILogService<TopicService> logService;
        private readonly IExcelExporter excelExporter;
        public TopicService(IDapperRepository dapperRepository,
            ICategoryService categoryService,
            ILogService<TopicService> logService,
            Repository<TopicEntity> topicRepository,
            Repository<TopicLikesEntity> topicLikeRepository,
            Repository<TopicFavoritesEntity> topicFavoriteRepository,
            Repository<TopicGoalEntity> topicTopicGoalRepository,
            FlowApplyProxyService flowApplyProxyService,
            IExcelExporter excelExporter
            )
        {
            this.logService = logService;
            this.dapperRepository = dapperRepository;
            this.topicRepository = topicRepository;
            this.categoryService = categoryService;
            this.topicLikeRepository = topicLikeRepository;
            this.topicFavoriteRepository = topicFavoriteRepository;
            this.excelExporter = excelExporter;
            this.flowApplyProxyService = flowApplyProxyService;
            this.topicTopicGoalRepository = topicTopicGoalRepository;
        }

        public List<SwaggerTopicApiParameters> GetSwaggerTopicApiParameters()
        {
            var client = topicRepository.AsSugarClient();
            var entities = client.Queryable<TopicEntity>().ToList();
            return entities.Select(t => new SwaggerTopicApiParameters
            {
                TopicId = t.id,
                Name = t.name,
                Descritption = t.description,
                Input = string.IsNullOrWhiteSpace(t.json_parameters_input) ? null : JsonConvert.DeserializeObject<ParametersInputSettingModel>(t.json_parameters_input),
                Output = string.IsNullOrWhiteSpace(t.json_parameters_output) ? null : JsonConvert.DeserializeObject<ParametersOutputSettingModel>(t.json_parameters_output),
            }).ToList();
        }

        private bool GetTopicAuthorizationUserFromCache(long topicId, string userId)
        {
            var client = topicRepository.AsSugarClient();
            var cacheKey = $"{MemoryCacheKeys.AuthorizeUserId}_{topicId}_{userId}";
            return MemoryCacheUtils.Get(cacheKey, TimeSpan.FromSeconds(60), () =>
            {
                var topicEntity = client.Queryable<TopicEntity>().Where(t => t.owner_id == userId && t.id == topicId).First();
                var authorizeEntity = client.Queryable<TopicAuthorizationUserEntity>().Where(t => t.topic_id == topicId && t.user_id == userId).First();

                return (topicEntity != null || authorizeEntity != null);
            });
        }

        public async Task<object> GetResultAsync(long id)
        {
            return await GetTopicResult(id, false);
        }

        public async Task<object> GetPreviewResultAsync(long id)
        {
            return await GetTopicResult(id, true);
        }
        public async Task<ActionResult> GetPreviewDataToExcel(long id)
        {
            var result = await GetTopicResult(id, true) ?? throw Oops.Oh(" No data.");
            var convertedData = new List<Dictionary<string, object>>();
            if (result is List<dynamic> pagedResultDto)
                convertedData = pagedResultDto.Adapt<List<Dictionary<string, object>>>();
            else
                convertedData.Add(result.Adapt<Dictionary<string, object>>());

            var table = new DataTable("PreviewTable");
            var header = convertedData.First();
            foreach (var item in header.Keys)
                table.Columns.Add(item, typeof(string));

            foreach (var item in convertedData)
                table.Rows.Add(item.Select(t => t.Value).ToArray());

            var bytes = await excelExporter.ExportAsByteArray(table);

            return new XlsxFileResult(bytes: bytes, fileDownloadName: $"topic_{id}");
        }

        private async Task<object> GetTopicResult(long id, bool isPreview)
        {
            var hasPermission = GetTopicAuthorizationUserFromCache(id, CurrentUser.Id);
            if (!hasPermission) throw Oops.Oh("permission denied.");
            var input = App.HttpContext.Request.Query;
            object result = new List<dynamic>();
            var topicEntity = await topicRepository.GetByIdAsync(id);
            if (topicEntity != null)
            {
                //Goal
                var topicGoalQuery = topicTopicGoalRepository.AsQueryable();
                var goalList = topicGoalQuery.Where(t => t.topic_id == topicEntity.id).ToList();
                try
                {
                    //The CommandText property has not been properly initialized
                    var parametersOutputSettings = string.IsNullOrWhiteSpace(topicEntity.json_parameters_output) ? new ParametersOutputSettingModel { Parameters = new List<ParametersOutputItemModel> { } } : JsonConvert.DeserializeObject<ParametersOutputSettingModel>(topicEntity.json_parameters_output);
                    var parametersInputSettings = string.IsNullOrWhiteSpace(topicEntity.json_parameters_input) ? new ParametersInputSettingModel { Parameters = new List<ParametersInputItemModel> { } } : JsonConvert.DeserializeObject<ParametersInputSettingModel>(topicEntity.json_parameters_input);
                    var inputCount = parametersInputSettings.Parameters.Where(t => input.Keys.Any(a => a.ToLower() == t.Name.ToLower())).Count();
                    if (!isPreview && parametersInputSettings.Parameters.Count != inputCount)
                        throw Oops.Oh("The number of input parameters does not match the required number of parameters");

                    if (string.IsNullOrWhiteSpace(topicEntity.sql_scripts))
                        throw Oops.Oh("The CommandText property has not been properly initialized");

                    var isPaged = false;
                    var whereString = string.Empty;
                    var sqlBuilder = new StringBuilder();
                    var querySql = topicEntity.sql_scripts;

                    if (parametersOutputSettings.IsPaged)
                    {
                        isPaged = true;
                    }

                    if (isPreview)
                    {
                        whereString = BuildWhereString(input, whereString);
                    }

                    BuildSql(whereString, sqlBuilder, querySql);

                    var sqlForPaged = sqlBuilder.ToString();
                    var queryParameters = QueryCollectionHelper.QueryToDictionary(input, parametersInputSettings.Parameters);
                    var pageIndex = input.GetPageIndex();
                    var pageSize = input.GetPageSize();
                    if (pageIndex != -1 && pageSize != -1)
                    {
                        isPaged = true;
                        var offset = (pageIndex - 1) * pageSize;

                        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1, "pageSize");
                        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 0, "offset");

                        sqlBuilder.AppendLine($"LIMIT {pageSize} offset {offset}");
                    }
                    else
                    {
                        sqlBuilder.AppendLine($"LIMIT 100 offset 0 ");
                    }

                    if (isPreview)
                    {
                        var ipageIndex = 1;
                        var ipageSize = 100;
                        var offset = 0;
                        querySql = topicEntity.sql_scripts;
                        queryParameters = QueryCollectionHelper.SetDefalutValue(parametersInputSettings.Parameters);
                        if (queryParameters.TryGetValue("pageindex", out object oPageIndex) && queryParameters.TryGetValue("pagesize", out object oPageSize))
                        {
                            isPaged = true;
                            ipageIndex = Convert.ToInt32(oPageIndex);
                            ipageSize = Convert.ToInt32(oPageSize);
                            offset = (ipageIndex - 1) * ipageSize;

                            ArgumentOutOfRangeException.ThrowIfLessThan(ipageSize, 1, "ipageSize");
                            ArgumentOutOfRangeException.ThrowIfLessThan(ipageSize, 0, "offset");
                        }

                        if (parametersOutputSettings.IsPaged)
                            sqlBuilder.AppendLine($"LIMIT {ipageSize} offset {offset}");
                        else
                            sqlBuilder.AppendLine($"LIMIT {ipageSize}");
                    }

                    var sqlForQuery = sqlBuilder.ToString();
                    sqlForQuery = SqlParamHandler.ReplaceParam(sqlForQuery, parametersInputSettings.Parameters, queryParameters);

                    //Split Statements
                    var sqlStatements = new List<string>();

                    // Add Paged Statement
                    if (isPaged)
                    {
                        var pagedSql = $"SELECT COUNT(*) as total_count from( {sqlForPaged} ) as t0;";
                        sqlStatements.Add(pagedSql);
                    }

                    // Add Query Statement
                    sqlStatements.Add(sqlForQuery);

                    var results = new List<dynamic>();
                    await SplitSql(queryParameters, sqlStatements.ToArray(), results);

                    var pagedResult = new DapperPagedResultDto();
                    if (results.Count > 0)
                    {
                        var totalCount = long.MinValue;
                        IEnumerable<dynamic> firstResult = results.FirstOrDefault();
                        IEnumerable<dynamic> secondResult = results.Count > 1 ? results.LastOrDefault() : null;
                        if (firstResult != null && secondResult != null)
                        {
                            if (firstResult.Count() == 1)
                            {
                                var first = firstResult.FirstOrDefault() as IDictionary<string, object>;
                                if (first.Keys.Count == 1 && first.TryGetValue("total_count", out object value))
                                    totalCount = (long)value;
                            }

                            if (totalCount != long.MinValue)
                            {
                                addGoalColumn(goalList, secondResult);

                                pagedResult.TotalCount = totalCount;
                                pagedResult.Data = secondResult;
                                result = pagedResult;
                            }
                            else
                            {
                                throw new NotImplementedException("Paging query is not implemented");
                            }
                        }
                        else if (firstResult.Any())
                        {
                            addGoalColumn(goalList, firstResult);

                            //result = firstResult.Count() == 1 ? firstResult.FirstOrDefault() : firstResult;
                            result = firstResult;
                        }
                    }

                    //REFERENCED TIMES
                    if (!isPreview)
                    {
                        topicEntity.referenced_times = topicEntity.referenced_times == null ? 1 : topicEntity.referenced_times + 1;
                        await topicRepository.AsUpdateable(topicEntity).UpdateColumns("referenced_times").ExecuteCommandAsync();
                    }
                }
                catch (Exception ex)
                {
                    await logService.LogErrorAsync($"Message=>{ex.Message},StackTrace={ex.StackTrace}");
                    throw new Exception(ex.Message, ex);
                }
            }

            return result;
        }

        private static void addGoalColumn(List<TopicGoalEntity> goalList, IEnumerable<dynamic> firstResult)
        {
            var _column_ = goalList?.FirstOrDefault()?.date_column;
            var _goal_column_ = "_goal_column_";
            var _goal_value = "_goal_value";
            foreach (var item in firstResult)
            {
                if (string.IsNullOrWhiteSpace(_column_)) continue;
                var aa = item as IDictionary<string, object>;
                aa.Add(_goal_column_, "scrap");
                aa.Add(_goal_value, 0.0);

                if (aa.TryGetValue(_column_, out object _monthValue) && DateTime.TryParse(_monthValue.ToString(), out DateTime _month))
                {
                    var goalItem = goalList.FirstOrDefault(t => t.month.Year == _month.Year && t.month.Month == _month.Month);
                    if (goalItem != null)
                    {
                        aa[_goal_column_] = goalItem.goal_column;
                        aa[_goal_value] = goalItem.goal;
                    }
                }
            }
        }

        private static string BuildWhereString(IQueryCollection input, string whereString)
        {
            List<string> whereList = new List<string>();
            foreach (var k in input.Keys)
            {
                if (k.StartsWith("field_", StringComparison.OrdinalIgnoreCase))
                {
                    whereList.Add($"{k.Replace("field_", "")}='{input[k]}'");
                }
            }
            if (whereList.Count > 0)
            {
                whereString = $" where {string.Join(" and ", whereList)}";
            }

            return whereString;
        }

        private static void BuildSql(string whereString, StringBuilder sqlBuilder, string querySql)
        {
            sqlBuilder.AppendLine("with cte as(");
            sqlBuilder.AppendLine(querySql);
            sqlBuilder.AppendLine(")");
            sqlBuilder.AppendLine("SELECT * from cte ");

            // Add where string
            if (whereString.Length > 0)
                sqlBuilder.AppendLine(whereString);
        }

        private async Task SplitSql(Dictionary<string, object> queryParameters, string[] sqlStatements, List<dynamic> results)
        {
            foreach (var sql in sqlStatements)
            {
                if (sql.Contains("create ", StringComparison.OrdinalIgnoreCase) 
                    || sql.Contains("delete ", StringComparison.OrdinalIgnoreCase)
                    || sql.Contains("truncate ", StringComparison.OrdinalIgnoreCase)
                    || sql.Contains("drop ", StringComparison.OrdinalIgnoreCase))
                    throw Oops.Oh("operations are not supported");

                if (sql.Contains("insert", StringComparison.OrdinalIgnoreCase) || sql.Contains("update", StringComparison.OrdinalIgnoreCase))
                {
                    await dapperRepository.Context.ExecuteAsync(sql, queryParameters);
                }
                else if (sql.Contains("select", StringComparison.OrdinalIgnoreCase))
                {
                    var queryResult = await dapperRepository.Context.QueryAsync(sql, queryParameters);
                    results.Add(queryResult);
                }
            }
        }

        public async Task<PagedResultDto<TopicListItemDto>> GetPagingListAsync(SearchTopicDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(dto.CategoryId);

            dto.PageIndex = dto.PageIndex <= 0 ? 1 : dto.PageIndex;
            dto.PageSize = dto.PageSize <= 10 ? 10 : dto.PageSize;

            var children = await categoryService.GetChildrenAsync(dto.CategoryId.Value);
            var query = topicRepository.AsQueryable();
            if (children is List<CategoryListItemDto> { Count: > 0 })
            {
                var categoryIds = children.Select(a => a.id);
                query = query.Where(t => categoryIds.Contains(t.category_id));
            }

            if (!string.IsNullOrWhiteSpace(dto.TopicName))
            {
                if (long.TryParse(dto.TopicName.Trim(), out long code))
                {
                    query = query.Where(t => t.id == code);
                }
                else
                {
                    query = query.WhereIF(!string.IsNullOrWhiteSpace(dto.TopicName), t => t.name.Contains(dto.TopicName));
                }
            }

            var totalCount = await query.CountAsync();
            var data = await query.OrderByDescending(t => t.created_time).ToPageListAsync(dto.PageIndex, dto.PageSize);
            var pagedResult = new PagedResultDto<TopicListItemDto>();

            pagedResult.TotalCount = totalCount;
            pagedResult.Data = data.Adapt<List<TopicListItemDto>>();

            return pagedResult;
        }

        public async Task<List<TopicListItemDto>> GetRankingListAsync(RankingDto input)
        {
            ArgumentNullException.ThrowIfNull(input);
            input.Count = input.Count < 1 ? 10 : input.Count;

            var query = topicRepository.AsQueryable();
            switch (input.RankingType)
            {
                case TopicRankingType.NewReleased:
                    return (await query.OrderByDescending(t => t.updated_time).Take(input.Count).ToListAsync()).Adapt<List<TopicListItemDto>>();
                case TopicRankingType.MostFavorite:
                    return (await query.OrderByDescending(t => t.favorite_count).Take(input.Count).ToListAsync()).Adapt<List<TopicListItemDto>>();
                case TopicRankingType.FastestGrowth:
                    return (await query.OrderByDescending(t => t.ratings).Take(input.Count).ToListAsync()).Adapt<List<TopicListItemDto>>();
                default:
                    break;
            }
            return null;
        }

        public async Task<bool> CheckIsExitsAsync(long id)
        {
            var entity = await topicRepository.GetByIdAsync(id);
            return entity != null;
        }

        public async Task<bool> UpdateRatingsAsync(UpdateTopicRatingsDto dto)
        {
            var entity = await topicRepository.GetByIdAsync(dto.id);
            if (entity != null)
            {
                entity.ratings = dto.ratings;
                entity.ratings_count = dto.ratings_count;
                topicRepository.AsSugarClient().Updateable(entity).UpdateColumns("ratings", "ratings_count").ExecuteCommand();
            }

            return true;
        }

        public async Task<TopicDetailsDto> GetDetailsAsync(long id)
        {
            var entity = await topicRepository.GetByIdAsync(id);
            _ = entity ?? throw Oops.Oh("Data Topic information does not exist.");
            var result = entity.Adapt<TopicDetailsDto>();
            var topicRatingsRepository = topicRepository.ChangeRepository<Repository<TopicRatingsEntity>>();
            var ratingsQuery = topicRatingsRepository.AsQueryable();
            var all = ratingsQuery.Where(t => t.topic_id == id).Count();
            var group = ratingsQuery.GroupBy(t => t.star).Select(t => new { t.star, count = SqlFunc.AggregateCount(t.star) }).ToList();

            result.star_ratio = new Dictionary<string, double>
            {
                ["1"] = 0,
                ["2"] = 0,
                ["3"] = 0,
                ["4"] = 0,
                ["5"] = 0,
            };
            result.star_ratio = group.ToDictionary(t => t.star.ToString(), t => Math.Round(((t.count * 100.0) / all), 2)).Adapt(result.star_ratio);
            var likeEntity = await topicLikeRepository.GetFirstAsync(t => t.topic_id == id && t.user_id == CurrentUser.Id);
            result.is_liked = likeEntity == null ? null : likeEntity.like_type == LikesType.Likes;

            var favoriteEntity = await topicFavoriteRepository.GetFirstAsync(t => t.topic_id == id && t.user_id == CurrentUser.Id);
            result.is_favorite = favoriteEntity == null ? false : true;
            //Ratings

            //Data Lineage
            result.datalineage_url = $"{OpenLineageAgent.GetOptions().VisableUrl}{result.datalineage_url}";

            var hasPermission = GetTopicAuthorizationUserFromCache(id, CurrentUser.Id);
            result.has_datapreview_permission = hasPermission;

            //Formual

            //Comments

            //Goal
            var topicGoalQuery = topicTopicGoalRepository.AsQueryable();
            var goalList = topicGoalQuery.Where(t => t.topic_id == id).ToList();
            if (goalList != null)
            {
                UnifyContext.Fill(new
                {
                    goals = goalList.Select(t => new { month = t.month.ToString("yyyy-MM"), t.goal, t.goal_column, t.date_column }).OrderByDescending(t => t.month)
                });
            }


            return result;
        }

        public long CreateSeqForTopicId()
        {
            var results = topicRepository.Context.SqlQueryable<CreateSeqForTopicIdDto>("select reset_daily_seq_for_topic_id as topic_id from reset_daily_seq_for_topic_id() ");
            return results.First().topic_id;
        }

        public async Task<bool> NewDataTopicAsync(NewTopicDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var currentUserId = CurrentUser.Id;
            var currentUserName = CurrentUser.Name;
            var department = CurrentUser.Department;
            var now = DateTime.Now;
            var client = topicRepository.AsSugarClient();

            try
            {
                var topic_id = CreateSeqForTopicId();
                var flow_id = Guid.NewGuid();
                await client.Ado.BeginTranAsync();
                _ = await client.Insertable(new TopicDraftEntity
                {
                    id = topic_id,
                    created_by = currentUserId,
                    created_time = now,
                    flow_id = flow_id,
                    department = department,
                    author = currentUserName,
                    author_id = currentUserId,
                    name = "New data topic",
                    content = input.Contents,
                    category_id = input.category_id,
                    version = "1",
                    email = CurrentUser.Email,
                    progress = ProgressStatus.ITDeveloping

                }).ExecuteCommandAsync();

                await client.Ado.CommitTranAsync();
                await CreateApproveFlow(input, topic_id, flow_id);
            }
            catch (Exception ex)
            {
                await client.Ado.RollbackTranAsync();
                await logService.LogErrorAsync($"{nameof(NewDataTopicAsync)},Message=>{ex.Message},StackTrace=>{ex.StackTrace}");
                throw;
            }

            return true;
        }

        private async Task CreateApproveFlow(NewTopicDto input, long topic_id,Guid flow_id)
        {
            var flowActApprovers = new List<ApproveFlow.Dto.FlowActApprover>();
            flowActApprovers.Add(new ApproveFlow.Dto.FlowActApprover
            {
                ActName = "SME Validation",
                ActorParms = new List<ApproveFlow.Dto.StaffInfo> {
                 new ApproveFlow.Dto.StaffInfo{ Ntid = CurrentUser.Id, Name = CurrentUser.Name, Department = CurrentUser.Department, Email = CurrentUser.Email }
                },
                ActorParmsName = CurrentUser.Name
            });

            // Approve flow
            var result = await flowApplyProxyService.InitFlowAsync(new ApproveFlow.Dto.StartFlowDto
            {
                FlowTempName = ApproveFlowConsts.BusinessModelApplication,
                Applicant = CurrentUser.Id,
                ApplicantName = CurrentUser.Name,
                FormId = flow_id,
                FormData = new
                {
                    category_id = input.category_id,
                    topic_id = topic_id.ToString(),
                    progress = nameof(ProgressStatus.ITDeveloping)
                },
                ActApprovers = flowActApprovers
            });

            if (!result.Success)
            {
                throw new Exception(result.Msg.ToString());
            }
        }
    }
}
