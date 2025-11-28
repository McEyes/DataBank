using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using DataQualityAssessment.Application.Assessments.Services;
using DataQualityAssessment.Application.DataAssets.Services;
using DataQualityAssessment.Application.Logs.Services;
using DataQualityAssessment.Application.QualityRating.Dtos;
using DataQualityAssessment.Application.RuleEngines;
using DataQualityAssessment.Application.RuleEngines.Rules;
using DataQualityAssessment.Application.Rules.Dtos;
using DataQualityAssessment.Application.Rules.Services;
using DataQualityAssessment.Core.Dappers;
using DataQualityAssessment.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataQualityAssessment.Application.QualityRating.Services
{
    public class QualityRatingService : IQualityRatingService, ITransient
    {
        private readonly ILogger<QualityRatingService> _logger;
        private readonly IAssessmentService _assessmentService;
        private readonly IDataAssetService _dataAssetService;
        private readonly ITableRuleService _tableRuleService;
        private readonly IExceptionLogService _lgger;
        private IServiceScopeFactory _scopeFactory;
        private IDbHelper db = null;
        private IDbConnection dbConnection = null;
        //private CancellationTokenSource cts = new CancellationTokenSource(1000 * 60 * 20);//20 min
        public QualityRatingService(
            ILogger<QualityRatingService> logger,
            IServiceScopeFactory scopeFactory,
            IExceptionLogService exceptionLog,
            IAssessmentService assessmentService,
            IDataAssetService dataAssetService,
            ITableRuleService tableRuleService
            )
        {
            _logger = logger;
            _lgger = exceptionLog;
            _assessmentService = assessmentService;
            _dataAssetService = dataAssetService;
            _tableRuleService = tableRuleService;
            _scopeFactory = scopeFactory;
            db = App.GetRequiredService<IDbHelper>();
        }

        public void CloseAllConnections()
        {
            db.CloseAllConnection();
        }

        public async Task EvaluateAsync(CancellationToken token)
        {
            //string[] tablesids = new string[] { "1846462298529640450", "1816017367701110785", "1764944472153522177" };
            var waitingList = (await _assessmentService.GetWaitingListAsync());//.Where(t => tablesids.Any(a=>a == t.Id));
            var size = 100;
            var total = waitingList.Count();
            var pageCount = total % size == 0 ? total / size : total / size + 1;

            await DoEvaluateAsync(waitingList, token);

            //TaskFactory factory = new TaskFactory(token);
            //List<Task> tasks = new List<Task>();

            //for (int page = 0; page < pageCount; page++)
            //{
            //    var data = waitingList.Skip(page * size).Take(size).ToList();
            //    var newTask = factory.StartNew(async () =>
            //    {
            //        await DoEvaluateAsync(data, token);
            //    }).Unwrap();
            //    tasks.Add(newTask);

            //    Thread.Sleep(10);
            //}

            //Task.WaitAll(tasks.ToArray());
        }

        private async Task DoEvaluateAsync(IEnumerable<Assessments.Dtos.AssessmentResultDto> waitingList, CancellationToken token)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                ILogger<QualityRatingService> logger = serviceProvider.GetRequiredService<ILogger<QualityRatingService>>();
                IExceptionLogService lgger = serviceProvider.GetRequiredService<IExceptionLogService>();
                IAssessmentService assessmentService = serviceProvider.GetRequiredService<IAssessmentService>();
                foreach (var waiting in waitingList)
                {
                    var tableId = waiting.Id;

                    try
                    {
                        logger.LogInformation($"=========Table {tableId} Start Evaluation=========");
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        await assessmentService.BeginEvaluateAsync(tableId);
                        var score = await EvaluateAsync(waiting.Id, token);
                        await assessmentService.EndEvaluateAsync(tableId, score);
                        stopwatch.Stop();
                        logger.LogInformation($"=========Table {tableId} Evaluation completed, score: {score}, time taken: {stopwatch.ElapsedMilliseconds} milliseconds=========");
                    }
                    catch (OperationCanceledException ex)
                    {
                        logger.LogWarning($"{ex.Message}");
                    }
                    catch (AggregateException ex) {
                        var exceptionMessage = $"Evaluate AggregateException tableId({tableId}): Message={ex.Message},StackTrace={ex.StackTrace}";
                        logger.LogError(exceptionMessage);
                        await assessmentService.SaveExceptionAsync(tableId, exceptionMessage);
                        await lgger.WriteAsync(nameof(QualityRatingService), nameof(EvaluateAsync), exceptionMessage);
                    }
                    catch (Exception ex)
                    {
                        var exceptionMessage = $"Evaluate Exception tableId({tableId}): Message={ex.Message},StackTrace={ex.StackTrace}";
                        logger.LogError(exceptionMessage);
                        await assessmentService.SaveExceptionAsync(tableId, exceptionMessage);
                        await lgger.WriteAsync(nameof(QualityRatingService), nameof(EvaluateAsync), $"Message={ex.Message},StackTrace={ex.StackTrace}");
                    }

                    token.ThrowIfCancellationRequested();
                }
            }
        }

        public async Task AddToEvaluateAsync(AddToEvaluateDto dto)
        {
            var data = await _tableRuleService.GetTableRulesAsync(dto.TableId);
            if (data == null || data.Count == 0)
                Oops.Oh("Please bind the rules first");
            if (data.Sum(t => t.Weight) != 100)
                Oops.Oh("The total number of bound rules must be equal to 100");

            await _assessmentService.AddToEvaluateAsync(dto.TableId);
        }

        private async Task<double> EvaluateAsync(string tableId, CancellationToken token)
        {
            (MetadataSourceModel sourceEntity, MetadataTableModel tableEntity, List<MetadataColumnModel> columnData) = await _dataAssetService.GetDataAssetAsync(tableId);

            ArgumentNullException.ThrowIfNull(tableEntity, "tableEntity");
            ArgumentNullException.ThrowIfNull(sourceEntity, "sourceEntity");
            ArgumentNullException.ThrowIfNull(columnData, "columnData");

            var dataAssetDbsetting = JsonConvert.DeserializeObject<DataassetDbSettingModel>(sourceEntity.db_schema);
            var dbSetting = new DbSettings(dataAssetDbsetting.host, dataAssetDbsetting.port, dataAssetDbsetting.dbName, dataAssetDbsetting.username, dataAssetDbsetting.password, tableEntity.id, tableEntity.table_name, sourceEntity.db_type);
            var engine = new RuleEngine();
            var ruleList = await _tableRuleService.GetRuleListAsync(tableId);
            var authorizeOwnerData = _dataAssetService.GetMetadataAuthorizeOwners(tableId);

            if (ruleList is List<TableRuleDto> { Count: > 0 })
            {
                var totalWeight = ruleList.Sum(t => t.Weight);
                if (totalWeight != 100)
                    throw new Exception($"The sum of the weight values of all detection rules must be equal to 100");

                dbConnection = db.CreateDbConnection(dbSetting);

                var context = new RuleDataContext
                {
                    DbConnection = dbConnection,
                    DbSetting = dbSetting,
                };

                foreach (var rule in ruleList)
                {
                    context.RuleSetting = new RuleSettings
                    {
                        Columns = columnData.Adapt<List<MetadataColumnModel>>(),
                        Table = tableEntity.Adapt<MetadataTableModel>(),
                        RuleNo = rule.RuleNo,
                        Weight = rule.Weight,
                    };

                    try
                    {
                        if(rule.ValidateType == Core.Enums.RuleValidateType.DataInformationIntegrity)
                        {
                            context.Data = authorizeOwnerData;
                        }

                        var newRule = RuleFactory.CreateRule(rule.ValidateType, context);
                        engine.AddRule(newRule);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Message={ex.Message},StackTrace={ex.StackTrace}");
                    }

                    token.ThrowIfCancellationRequested();
                }

                var score = await engine.EvaluateAsync(token);
                return score;

            }
            else
            {
                throw new Exception($"Rule not specified");
            }
        }
    }
}
