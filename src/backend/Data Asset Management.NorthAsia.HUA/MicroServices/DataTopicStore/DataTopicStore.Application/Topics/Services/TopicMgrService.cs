
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using DataTopicStore.Application.ApproveFlow;
using DataTopicStore.Application.ApproveFlow.Dtos;
using DataTopicStore.Application.AuthProxy.Dtos;
using DataTopicStore.Application.Categories.Dtos;
using DataTopicStore.Application.Categories.Services;
using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Helpers;
using DataTopicStore.Application.Logs.Services;
using DataTopicStore.Application.SchemaTables.Services;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Application.UserProxy;
using DataTopicStore.Core.Datalineage.Openlineage;
using DataTopicStore.Core.Datalineage.Openlineage.Events;
using DataTopicStore.Core.Datalineage.Openlineage.Models;
using DataTopicStore.Core.Datalineage.Openlineage.Models.ColumnLineages;
using DataTopicStore.Core.Datalineage.Sqllineage;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Enums;
using DataTopicStore.Core.Extensions;
using DataTopicStore.Core.Models;
using DataTopicStore.Core.Repositories;
using Dm;
using Dm.util;
using Mapster;
using MySqlX.XDevAPI.Relational;
using Newtonsoft.Json;
using Npgsql.Logging;
using SqlSugar.Extensions;
using static Dapper.SqlMapper;
using static DataTopicStore.Core.Datalineage.Openlineage.Dtos.CreateDatasetDto;

namespace DataTopicStore.Application.Topics.Services
{
    public class TopicMgrFormCheckService
    {
        public static void CheckItDevelopRecordForm(TopicDraftEntity input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));

            CreateITDevelopingModel itDevelop = JsonConvert.DeserializeObject<CreateITDevelopingModel>(input.json_develop_records ?? string.Empty) ?? new CreateITDevelopingModel();
            if (itDevelop.APIGeneration == null || itDevelop.APIGeneration.pic_ntid == null) throw Oops.Oh("API Generation is required.");
            if (itDevelop.DataSourceDefinition == null || itDevelop.DataSourceDefinition.pic_ntid == null) throw Oops.Oh("Data Source Definition is required.");
            if (itDevelop.DataSourceIngestion == null || itDevelop.DataSourceIngestion.pic_ntid == null) throw Oops.Oh("Data Source Ingestion is required.");
            if (itDevelop.PhysicalModeling == null || itDevelop.PhysicalModeling.pic_ntid == null) throw Oops.Oh("Physical Modeling is required.");
            if (itDevelop.TScriptGeneration == null || itDevelop.TScriptGeneration.pic_ntid == null) throw Oops.Oh("T-Script Generation is required.");

        }

        public static void CheckOutputParameterForm(TopicDraftEntity input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));
            ArgumentNullException.ThrowIfNull(input, nameof(input));

            if (string.IsNullOrWhiteSpace(input.json_parameters_output)) throw Oops.Oh("Output Parameters is required.");
            var parametersOutputSettings = JsonConvert.DeserializeObject<ParametersOutputSettingModel>(input.json_parameters_output);
            if (parametersOutputSettings == null) throw Oops.Oh("Output Parameters is required.");
            if (string.IsNullOrWhiteSpace(parametersOutputSettings.Type)) throw Oops.Oh("Output Parameters[Type] is required.");
        }

        public static void CheckVerifyForm(TopicDraftEntity input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(input.sql_scripts, "sql_scripts");
            if (!input.is_verification_passed)
            {
                throw Oops.Oh("Unverified.");
            }
        }

        public static void CheckIsPublishedForm(TopicDraftEntity input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));
            if (input.is_published != true)
            {
                throw new Exception("You can only operate after it is published.");
            }
        }
    }
    public class TopicMgrService : ApplicationService, ITopicMgrService
    {
        private readonly Repository<TopicEntity> topicRepository;
        private readonly Repository<TopicHistoryEntity> topichistoryRepository;
        private readonly Repository<TopicDraftEntity> topicDraftRepository;
        private readonly Repository<TopicAuthorizationUserEntity> topicAuthorizationRepository;
        private readonly Repository<TopicAccessRequestEntity> topicAccessRequestRepository;
        private readonly Repository<SchemaColumnsEntity> schemaRequestRepository;
        private readonly FlowApplyProxyService flowApplyProxyService;
        private readonly AuthProxyService authProxyService;
        private readonly IDapperRepository dapperRepository;
        private readonly ILogService<TopicMgrService> logService;
        private readonly ICategoryService categoryService;
        private readonly ISchemaTablesService schemaTablesService;

        public TopicMgrService(
            ILogService<TopicMgrService> logService,
            IDapperRepository dapperRepository,
            Repository<TopicEntity> topicRepository,
            Repository<TopicHistoryEntity> topichistoryRepository,
            Repository<TopicDraftEntity> topicDraftRepository,
            Repository<TopicAuthorizationUserEntity> topicAuthorizationRepository,
            Repository<TopicAccessRequestEntity> topicAccessRequestRepository,
            Repository<SchemaColumnsEntity> schemaRequestRepository,
            FlowApplyProxyService flowApplyProxyService,
            AuthProxyService authProxyService,
            ICategoryService categoryService,
            ISchemaTablesService schemaTablesService)
        {
            this.logService = logService;
            this.dapperRepository = dapperRepository;
            this.topicRepository = topicRepository;
            this.topichistoryRepository = topichistoryRepository;
            this.topicDraftRepository = topicDraftRepository;
            this.categoryService = categoryService;
            this.topicAuthorizationRepository = topicAuthorizationRepository;
            this.topicAccessRequestRepository = topicAccessRequestRepository;
            this.schemaTablesService = schemaTablesService;
            this.schemaRequestRepository = schemaRequestRepository;
            this.flowApplyProxyService = flowApplyProxyService;
            this.authProxyService = authProxyService;
        }

        public async Task<bool> SaveAsync(CreateOrUpdateTopicDto input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));
            var entity = input.Adapt<TopicDraftEntity>();
            if (entity.id == 0) throw Oops.Oh("id cannot be 0");

            var originalEntity = await topicDraftRepository.GetByIdAsync(entity.id) ?? throw Oops.Oh("Topic information does not exit");
            var progressBefore = originalEntity.progress;
            var originalSqlscripts = originalEntity.sql_scripts;
            var ownerInfo = await authProxyService.GetEmployeeAsync(input.owner_id);
            if (ownerInfo == null || !ownerInfo.Success || ownerInfo.Data == null) throw Oops.Oh("Owner information does not exist.");

            originalEntity = input.Adapt(originalEntity);
            if (progressBefore == ProgressStatus.Completed)
            {
                originalEntity.sql_scripts = originalSqlscripts;
            }

            originalEntity.owner_id = ownerInfo.Data.WorkNTID;
            originalEntity.owner = ownerInfo.Data.Name;
            originalEntity.owner_email = ownerInfo.Data.WorkEmail;
            originalEntity.tags = input.tagList;
            originalEntity.updated_by = CurrentUser.Id;
            originalEntity.updated_time = DateTime.Now;
            await logService.LogInformationAsync($"{JsonConvert.SerializeObject(input)}", nameof(SaveAsync));

            var isSeccess = false;
            var ctx = topicRepository.Context;
            try
            {
                await ctx.Ado.BeginTranAsync();
                var topicEntity = await topicRepository.GetByIdAsync(entity.id);
                if (topicEntity != null)
                {
                    topicEntity.category_id = input.category_id;
                    topicEntity.tags = input.tagList;
                    topicEntity.json_formula = input.json_formula;
                    topicEntity.owner_id = ownerInfo.Data.WorkNTID;
                    topicEntity.owner = ownerInfo.Data.Name;
                    topicEntity.owner_email = ownerInfo.Data.WorkEmail;

                    await topicRepository.UpdateAsync(topicEntity);
                }

                await topicDraftRepository.UpdateAsync(originalEntity);
                await ctx.Ado.CommitTranAsync();

                isSeccess = true;
            }
            catch (Exception ex)
            {
                isSeccess = false;
                await ctx.Ado.RollbackTranAsync();
                await logService.LogErrorAsync($"Message={ex.Message},StackTrace={ex.StackTrace}", nameof(PublishAsync));
                throw;
            }

            return isSeccess;
        }

        public async Task<bool> PublishAsync(LongIdInputDto input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));
            var entity = await topicDraftRepository.GetByIdAsync(input.id);
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            if (!entity.is_verification_passed) throw Oops.Oh("Please confirm that you have passed the verification");
            ArgumentNullException.ThrowIfNull(entity.flow_id);
            var approveResult = await flowApplyProxyService.GetFlowInstAsync(entity.flow_id.Value);
            if (!approveResult.Success) throw Oops.Oh("Failed to retrieve approval workflow information");
            var approver = approveResult.Data.approver.Where(t => t.ntid.Contains(CurrentUser.Id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (approver == null)
            {
                throw Oops.Oh("You do not have permission to publish.");
            }

            //set is published
            entity.is_published = true;
            entity.progress = ProgressStatus.Publish;

            var topicEntity = entity.Adapt<TopicEntity>();
            topicEntity.updated_by = CurrentUser.Id;
            topicEntity.updated_time = DateTime.Now;
            var isSeccess = false;
            var oldEntity = await topicRepository.GetByIdAsync(entity.id);
            var ctx = topicRepository.Context;

            try
            {
                await ctx.Ado.BeginTranAsync();
                if (oldEntity == null)
                {
                    isSeccess = await topicRepository.InsertAsync(topicEntity);
                }
                else
                {
                    topicEntity.ratings = oldEntity.ratings;
                    topicEntity.referenced_times = oldEntity.referenced_times;
                    topicEntity.comments_count = oldEntity.comments_count;
                    topicEntity.favorite_count = oldEntity.favorite_count;
                    topicEntity.ratings_count = oldEntity.ratings_count;

                    var adaConfig = new TypeAdapterConfig();
                    adaConfig.NewConfig<TopicEntity, TopicHistoryEntity>().Ignore(t => t.id);

                    var historyEntity = oldEntity.Adapt<TopicHistoryEntity>(adaConfig);
                    historyEntity.topic_id = oldEntity.id;
                    historyEntity.id = Guid.NewGuid();
                    historyEntity.is_published = true;

                    await topichistoryRepository.InsertAsync(historyEntity);
                    await topicRepository.UpdateAsync(topicEntity);
                }

                if (entity.flow_id != null)
                {
                    var flowResult = await flowApplyProxyService.SendApproveAsync(new ApproveFlow.Dto.FlowAuditDto
                    {
                        AuditContent = "",
                        FlowInstId = entity.flow_id.Value
                    });

                    if (!flowResult.Success)
                    {
                        throw new Exception(flowResult.Msg.ToString());
                    }

                    await topicDraftRepository.UpdateAsync(entity);
                }

                await ctx.Ado.CommitTranAsync();

                isSeccess = true;
            }
            catch (Exception ex)
            {
                isSeccess = false;
                await ctx.Ado.RollbackTranAsync();
                await logService.LogErrorAsync($"Message={ex.Message},StackTrace={ex.StackTrace}", nameof(PublishAsync));
                throw;
            }

            if (isSeccess)
            {
                await SendDatalineage(entity);
            }

            await logService.LogInformationAsync($"{JsonConvert.SerializeObject(input)}", nameof(PublishAsync));
            return isSeccess;
        }

        public async Task<TopicDraftDetailsDto> GetTopicDraftInfoAsync(long id)
        {
            var entity = await topicDraftRepository.GetByIdAsync(id);
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            ArgumentNullException.ThrowIfNull(entity.flow_id);
            var result = entity.Adapt<TopicDraftDetailsDto>();
            var approveResult = await flowApplyProxyService.GetFlowInstAsync(entity.flow_id.Value);
            if (approveResult.Success && approveResult.Data != null && approveResult.Data.approver != null)
            {
                var approver = approveResult.Data.approver.Where(t => t.ntid.Contains(CurrentUser.Id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (approver != null) result.can_edit = true;
            }
            entity.json_develop_records = JsonConvert.SerializeObject(entity.json_develop_records);
            return result;
        }

        public async Task<object> VerifyAsync(long id, IQueryCollection input)
        {
            var isSeccess = true;
            var verifyMessage = string.Empty;
            var result = new object();
            var topicEntity = await topicDraftRepository.GetByIdAsync(id) ?? throw Oops.Oh("Can not found topic inforation.");
            ArgumentNullException.ThrowIfNullOrWhiteSpace(topicEntity.sql_scripts, "sql_scripts");
            ArgumentNullException.ThrowIfNull(topicEntity.flow_id);

            var approveResult = await flowApplyProxyService.GetFlowInstAsync(topicEntity.flow_id.Value);
            if (!approveResult.Success) throw Oops.Oh("Failed to retrieve approval workflow information");
            if (!CurrentUser.Id.Equals(approveResult.Data.applicant, StringComparison.OrdinalIgnoreCase))
            {
                throw Oops.Oh("You do not have permission to verify.");
            }

            var parametersOutputSettings = new ParametersOutputSettingModel { };
            var parametersInputSettings = new ParametersInputSettingModel { };
            if (!string.IsNullOrWhiteSpace(topicEntity.json_parameters_input))
                parametersInputSettings = JsonConvert.DeserializeObject<ParametersInputSettingModel>(topicEntity.json_parameters_input);

            if (!string.IsNullOrWhiteSpace(topicEntity.json_parameters_output))
                parametersOutputSettings = JsonConvert.DeserializeObject<ParametersOutputSettingModel>(topicEntity.json_parameters_output);

            parametersInputSettings.Parameters ??= [];

            var isPaged = false;
            var whereString = string.Empty;
            var sqlBuilder = new StringBuilder();
            var querySql = topicEntity.sql_scripts;
            var pageSize = 100;
            var pageIndex = 1;
            var offset = 0;
            BuildSql(whereString, sqlBuilder, querySql);

            var sqlForPaged = sqlBuilder.ToString();
            var queryParameters = QueryCollectionHelper.SetDefalutValue(parametersInputSettings.Parameters);
            if (queryParameters.TryGetValue("pageindex", out object oPageIndex) && queryParameters.TryGetValue("pagesize", out object oPageSize))
            {
                isPaged = true;
                pageIndex = Convert.ToInt32(oPageIndex);
                pageSize = Convert.ToInt32(oPageSize);
                offset = (pageIndex - 1) * pageSize;
            }

            if (parametersOutputSettings.IsPaged)
                sqlBuilder.AppendLine($"LIMIT {pageSize} offset {offset}");
            else
                sqlBuilder.AppendLine($"LIMIT {pageSize}");

            var sqlForQuery = sqlBuilder.ToString();
            sqlForQuery = SqlParamHandler.ReplaceParam(sqlForQuery, parametersInputSettings.Parameters, queryParameters);

            var sqlStatements = new List<string>();

            // Add Paged Statement
            if (isPaged)
            {
                var pagedSql = $"SELECT COUNT(*) as total_count from( {sqlForPaged} ) as t0;";
                sqlStatements.Add(pagedSql);
            }

            // Add Query Statement
            sqlStatements.Add(sqlForQuery);

            try
            {
                var results = new List<dynamic>();
                await SplitSql(queryParameters, sqlStatements.ToArray(), results);
                (result, isSeccess) = await GetQueryResults(id, results);
                await SaveValidationResult(isSeccess, string.Empty, topicEntity);
            }
            catch (Exception ex)
            {
                await logService.LogErrorAsync($"Message=>{ex.Message},StackTrace={ex.StackTrace}");
                await SaveValidationResult(false, ex.Message, topicEntity);
                throw new Exception(ex.Message, ex);
            }

            await SaveValidationResultAsync(new BusinessModelValidationResultDto
            {
                topic_id = id,
                Result = new BusinessModelValidationResult { Data = result }
            });

            return result;
        }

        private async Task SaveValidationResult(bool isSeccess, string failureReason, TopicDraftEntity topicEntity)
        {
            topicEntity.is_verification_passed = isSeccess;
            topicEntity.verification_failure_reason = failureReason;
            await topicDraftRepository.AsSugarClient().Updateable(topicEntity).UpdateColumns("is_verification_passed", "verification_failure_reason").ExecuteCommandAsync();
        }

        private async Task<(object, bool)> GetQueryResults(long id, List<dynamic> results)
        {
            var result = new object();
            var isSeccess = false;
            var pagedResult = new DapperPagedResultDto();
            if (results.Count > 0)
            {
                var totalCount = long.MinValue;
                IEnumerable<dynamic> firstResult = results.FirstOrDefault();
                IEnumerable<dynamic> secondResult = results.Count > 1 ? results.LastOrDefault() : null;

                var outputParametersDto = new SetParametersOutputDto
                {
                    TopicId = id,
                    OutputSetting = new ParametersOutputSettingModel
                    {
                        Type = SwaggerDataType.output_type_object,
                        Parameters = []
                    }
                };

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
                        IDictionary<string, object> queryResult = secondResult.FirstOrDefault() as IDictionary<string, object>;
                        pagedResult.TotalCount = totalCount;
                        pagedResult.Data = secondResult;
                        result = pagedResult;
                        outputParametersDto.OutputSetting.Type = SwaggerDataType.output_type_array;
                        outputParametersDto.OutputSetting.IsPaged = true;
                        SetParam(outputParametersDto, queryResult);
                        await SetParametersOutputAsync(outputParametersDto);
                    }
                    else
                    {
                        throw new NotImplementedException("Paging query is not implemented");
                    }

                    isSeccess = true;
                }
                else if (firstResult.Any())
                {
                    if (firstResult.Count() == 1)
                    {
                        result = firstResult.FirstOrDefault();
                        outputParametersDto.OutputSetting.Type = SwaggerDataType.output_type_object;
                    }
                    else
                    {
                        result = firstResult.Take(10);
                        outputParametersDto.OutputSetting.Type = SwaggerDataType.output_type_array;
                    }

                    IDictionary<string, object> queryResult = firstResult.FirstOrDefault() as IDictionary<string, object>;
                    SetParam(outputParametersDto, queryResult);
                    await SetParametersOutputAsync(outputParametersDto);

                    isSeccess = true;
                }
                else
                {
                    throw new NotImplementedException(" No Results");
                }
            }

            return (result, isSeccess);
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

        private async Task SendDatalineage(TopicDraftEntity topicEntity)
        {
            //Send Data lineage
            SqllineageAgent sqllineageAgent = new SqllineageAgent();
            OpenLineageAgent openLineageAgent = new OpenLineageAgent();

            var jobName = $"topic{topicEntity.id}";
            var tables = await sqllineageAgent.ExtractTables(topicEntity.sql_scripts);
            var wapperScript = $"insert into _default_.{jobName} {topicEntity.sql_scripts}";
            var columnlineageFields = await sqllineageAgent.ExtractColumnLineages(wapperScript);
            var jobNamespace = "datatopicstore";
            var datalineageUrl = $"/lineage/job/{jobNamespace}/{jobName}";
            var runEvent = RunEvent.CreateInstance(jobNamespace, jobName);
            if (tables.SourceTables is List<string> { Count: > 0 }
            && tables.IntermediateTables is List<string> { Count: > 0 }
            && tables.TargetTables is List<string> { Count: > 0 })
            {
                await EmitLineageEvent(openLineageAgent, runEvent, jobNamespace, jobName, tables.SourceTables, tables.IntermediateTables, columnlineageFields);
                runEvent.Job.Name = $"{jobName}_2";
                await EmitLineageEvent(openLineageAgent, runEvent, jobNamespace, jobName, tables.IntermediateTables, tables.TargetTables, columnlineageFields);
            }
            else if (tables.IntermediateTables is List<string> { Count: 0 })
            {
                await EmitLineageEvent(openLineageAgent, runEvent, jobNamespace, jobName, tables.SourceTables, tables.TargetTables, columnlineageFields);
            }

            var topic = new TopicEntity { id = topicEntity.id, datalineage_url = datalineageUrl };
            var topicDraft = new TopicDraftEntity { id = topicEntity.id, datalineage_url = datalineageUrl };

            await topicDraftRepository.AsSugarClient().Updateable(topicDraft).UpdateColumns("datalineage_url").ExecuteCommandAsync();
            await topicDraftRepository.AsSugarClient().Updateable(topic).UpdateColumns("datalineage_url").ExecuteCommandAsync();

            var sqlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"sqllineage -e \"{topicEntity.sql_scripts}\""));
            await logService.LogInformationAsync($"{sqlBase64}", nameof(SendDatalineage));
        }

        public async Task EmitLineageEvent(OpenLineageAgent openLineageAgent,
            RunEvent runEvent,
            string jobNamespace,
            string jobName,
            List<string> sourceTables, List<string> targetTables, Dictionary<string, object> columnlineageFields)
        {
            if (sourceTables is List<string> { Count: > 0 } && targetTables is List<string> { Count: > 0 })
            {
                foreach (var item in sourceTables)
                {
                    runEvent.Inputs.Add(new InputDataset
                    {
                        Namespace = jobNamespace,
                        Name = item
                    });
                }

                //foreach (var item in targetTables)
                //{
                //    var outputDs = new OutputDataset
                //    {
                //        Namespace = jobNamespace,
                //        Name = item
                //    };

                //    SetColumnlineage(outputDs, columnlineageFields);
                //    runEvent.Outputs.Add(outputDs);
                //}

            }
            else if (sourceTables is List<string> { Count: > 0 })
            {
                foreach (var item in sourceTables)
                {
                    runEvent.Inputs.Add(new InputDataset
                    {
                        Namespace = jobNamespace,
                        Name = item
                    });
                }

                //await openLineageAgent.EmitEvent(runEvent);
            }
            else
            {
                //foreach (var item in targetTables)
                //{
                //    var outputDs = new OutputDataset
                //    {
                //        Namespace = jobNamespace,
                //        Name = item
                //    };

                //    SetColumnlineage(outputDs, columnlineageFields);

                //    runEvent.Outputs.Add(outputDs);
                //}


                //await openLineageAgent.EmitEvent(runEvent);
            }



            if (targetTables == null || targetTables.Count == 0)
            {
                var dataset = $"_default_.{jobName}";
                var outputDs = new OutputDataset
                {
                    Namespace = jobNamespace,
                    Name = dataset,
                    Facets = new Dictionary<string, object>()
                };
                SetColumnlineage(outputDs, columnlineageFields);
                runEvent.Outputs.Add(outputDs);

                var datafields = new List<LineageNode_Data_Field>();
                if (columnlineageFields.Keys.Count > 0)
                {
                    foreach (var item in columnlineageFields.Keys)
                    {
                        var fieldName = item.Substring(item.LastIndexOf(".") + 1);
                        datafields.Add(new LineageNode_Data_Field
                        {
                            name = fieldName,
                            type = "VARCHAR",
                            description = fieldName
                        });
                    }
                }

                // Create Target Table
                await openLineageAgent.CreateDataset(jobNamespace, dataset, new Core.Datalineage.Openlineage.Dtos.CreateDatasetDto
                {
                    type = "DB_TABLE",
                    physicalName = dataset.ToLower(),
                    description = "",
                    fields = datafields,
                    sourceName = "default"
                });
            }

            await openLineageAgent.EmitEvent(runEvent);
            var schemaColumn = schemaRequestRepository.AsQueryable();

            // Create Dataset
            if (sourceTables is List<string> { Count: > 0 })
            {
                var concatTables = sourceTables.Concat(targetTables).Distinct();
                foreach (var ds in concatTables)
                {
                    if (string.IsNullOrWhiteSpace(ds) || ds.Split(".").Length != 2)
                        continue;
                    var split = ds.Split(".");
                    var table_schema = split[0];
                    var table_name = split[1];
                    var info = this.schemaTablesService.GetInfoFromCache(split[0], split[1]);
                    if (info != null)
                    {
                        schemaColumn = schemaRequestRepository.AsQueryable();
                        var fields = new List<LineageNode_Data_Field>();
                        var columns = schemaColumn.Where(t => t.table_schema.ToLower() == table_schema.ToLower() && t.table_name == table_name.ToLower());
                        if (columns != null && columns.Count() > 0)
                        {
                            fields = columns.Select(t => new LineageNode_Data_Field
                            {
                                name = t.column_name,
                                type = t.data_type,
                                description = t.column_comment
                            }).ToList();
                        }
                        await openLineageAgent.CreateDataset(jobNamespace, ds, new Core.Datalineage.Openlineage.Dtos.CreateDatasetDto
                        {
                            type = "DB_TABLE",
                            physicalName = ds.ToLower(),
                            description = info.table_comment,
                            fields = fields,
                            sourceName = "default"
                        });
                    }
                }
            }

            await logService.LogInformationAsync($"{JsonConvert.SerializeObject(runEvent)}", nameof(EmitLineageEvent));
        }

        private static void SetColumnlineage(OutputDataset dataset, Dictionary<string, object> columnLineageFields)
        {
            var columnFacet = new ColumnLineageFacet
            {
                _producer = "https://openlineage.io/spec/1-0-5/SchemaDatasetFacet.json",
                _schemaURL = "https://openlineage.io/spec/1-0-5/SchemaDatasetFacet.json",
                fields = columnLineageFields
            };

            var fields = columnLineageFields.Keys.Select(t => new LineageNode_Data_Field { type = "VARCHAR", name = t, description = t }).ToList();
            dataset.Facets.Add("schema", new Dictionary<string, object>
            {
                { "_producer","https://openlineage.io/spec/1-0-5/SchemaDatasetFacet.json"},
                { "_schemaURL","https://openlineage.io/spec/1-0-5/SchemaDatasetFacet.json"},
                { "fields",fields }
            });

            dataset.Facets.Add("columnLineage", columnFacet);
        }

        private static void SetParam(SetParametersOutputDto outputParametersDto, IDictionary<string, object> queryResult)
        {
            foreach (var kv in queryResult)
            {
                var param = new ParametersOutputItemModel { Name = kv.Key, Description = kv.Key, DataType = SwaggerDataType.data_type_string };
                if (kv.Value != null)
                {
                    var dataType = kv.Value.GetType().Name.ToLower();
                    switch (dataType)
                    {
                        case "sbyte":
                        case "int16":
                        case "int32":
                        case "int64":
                            param.DataType = SwaggerDataType.data_type_integer;
                            break;
                        case "decimal":
                        case "float":
                        case "double":
                            param.DataType = SwaggerDataType.data_type_number;
                            break;
                        case "string":
                            param.DataType = SwaggerDataType.data_type_string;
                            break;
                        case "datetime":
                            {
                                param.DataType = SwaggerDataType.data_type_string;
                                param.Format = SwaggerDataFormat.data_format_date_time;
                            }
                            break;
                        case "timespan":
                            param.DataType = SwaggerDataType.data_type_integer;
                            break;
                        case "bool":
                            param.DataType = SwaggerDataType.data_type_boolean;
                            break;
                        case "guid":
                            param.DataType = SwaggerDataType.data_type_string;
                            break;
                        default:
                            break;
                    }
                }

                outputParametersDto.OutputSetting.Parameters.Add(param);
            }
        }

        public async Task<bool> SaveValidationResultAsync(BusinessModelValidationResultDto input)
        {
            var entity = await topicDraftRepository.GetByIdAsync(input.topic_id);
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            entity.progress = ProgressStatus.Validation;
            entity.json_validation_result = JsonConvert.SerializeObject(input.Result);

            var columns = new string[] { nameof(entity.progress), nameof(entity.json_validation_result) };
            await topicDraftRepository.AsUpdateable(entity).UpdateColumns(columns).ExecuteCommandAsync();
            //logService.LogInformation(JsonConvert.SerializeObject(input), nameof(SaveValidationResultAsync));
            return true;
        }

        public async Task<bool> SaveITDevelopRecordsAsync(CreateITDevelopingDto input)
        {
            var entity = await topicDraftRepository.GetByIdAsync(input.topic_id);
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            entity.json_develop_records = JsonConvert.SerializeObject(input.Data);

            var columns = new string[] { nameof(entity.progress), nameof(entity.json_develop_records) };
            var effectRows = await topicDraftRepository.AsUpdateable(entity).UpdateColumns(columns).ExecuteCommandAsync();
            await logService.LogInformationAsync(JsonConvert.SerializeObject(input), nameof(SaveITDevelopRecordsAsync));
            return effectRows > 0;
        }

        public async Task<bool> SetParametersInputAsync(SetParametersInputDto input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));
            ArgumentNullException.ThrowIfNull(input.InputSetting, nameof(input.InputSetting));
            if (input.TopicId == 0) throw Oops.Oh("id cannot be 0");

            foreach (var item in input.InputSetting.Parameters)
            {
                if (!SwaggerDataType.DataTypes.Contains(item.DataType))
                    throw Oops.Oh($"The data type of the parameter {item.Name} is illegal and must be one of [{string.Join(",", SwaggerDataType.DataTypes)}] types.");

                if (SwaggerDataFormat.DataFmts.TryGetValue(item.Name, out List<string> value) && !value.Contains(item.Format))
                    throw Oops.Oh($"The data format of the parameter {item.Name} is incorrect. It must be one of {string.Join(",", value)}.");
            }

            var originalEntity = await topicDraftRepository.GetByIdAsync(input.TopicId) ?? throw Oops.Oh("Topic information does not exit");
            originalEntity = input.Adapt(originalEntity);
            originalEntity.updated_by = CurrentUser.Id;
            originalEntity.updated_time = DateTime.Now;
            originalEntity.json_parameters_input = JsonConvert.SerializeObject(input.InputSetting);
            string[] updateColumns = ["json_parameters_input", "updated_by", "updated_time"];
            var effectRows = await topicDraftRepository.AsUpdateable(originalEntity).UpdateColumns(updateColumns).ExecuteCommandAsync();
            await logService.LogInformationAsync(JsonConvert.SerializeObject(input), nameof(SetParametersInputAsync));
            return effectRows > 0;
        }

        public async Task<bool> SetParametersOutputAsync(SetParametersOutputDto input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));
            ArgumentNullException.ThrowIfNull(input.OutputSetting, nameof(input.OutputSetting));
            if (input.TopicId == 0) throw Oops.Oh("id cannot be 0");

            var outputTypes = new string[] { SwaggerDataType.output_type_object, SwaggerDataType.output_type_array };
            if (!outputTypes.Contains(input.OutputSetting.Type)) throw Oops.Oh("The type must be one of object and array");
            ArgumentNullException.ThrowIfNull(input.OutputSetting, nameof(input.OutputSetting));

            foreach (var item in input.OutputSetting.Parameters)
            {
                if (!SwaggerDataType.DataTypes.Contains(item.DataType))
                    throw Oops.Oh($"The data type of the parameter {item.Name} is illegal and must be one of [{string.Join(",", SwaggerDataType.DataTypes)}] types.");
            }

            var originalEntity = await topicDraftRepository.GetByIdAsync(input.TopicId) ?? throw Oops.Oh("Topic information does not exit");
            originalEntity.updated_by = CurrentUser.Id;
            originalEntity.updated_time = DateTime.Now;
            originalEntity.json_parameters_output = JsonConvert.SerializeObject(input.OutputSetting);
            string[] updateColumns = ["json_parameters_output", "updated_by", "updated_time"];
            var effectRows = await topicDraftRepository.AsUpdateable(originalEntity).UpdateColumns(updateColumns).ExecuteCommandAsync();
            await logService.LogInformationAsync(JsonConvert.SerializeObject(input), nameof(SetParametersOutputAsync));
            return effectRows > 0;
        }

        public async Task<bool> SetVerificationPassedAsync(SetVerificationPassedDto input)
        {
            var entity = await topicDraftRepository.GetByIdAsync(input.TopicId) ?? throw Oops.Oh("Topic information does not exit");
            entity.is_verification_passed = true;
            entity.updated_by = CurrentUser.Id;
            entity.updated_time = DateTime.Now;
            string[] updateColumns = ["is_verification_passed", "updated_by", "updated_time"];
            var effectRows = await topicDraftRepository.AsUpdateable(entity).UpdateColumns(updateColumns).ExecuteCommandAsync();
            await logService.LogInformationAsync(JsonConvert.SerializeObject(input), nameof(SetVerificationPassedAsync));
            return effectRows > 0;
        }

        public async Task<bool> SetVerificationFailureAsync(SetVerificationFailureDto input)
        {
            var entity = await topicDraftRepository.GetByIdAsync(input.TopicId) ?? throw Oops.Oh("Topic information does not exit");
            entity.is_verification_passed = false;
            entity.verification_failure_reason = input.Reason;
            entity.updated_by = CurrentUser.Id;
            entity.updated_time = DateTime.Now;
            string[] updateColumns = ["is_verification_passed", "verification_failure_reason", "updated_by", "updated_time"];
            var effectRows = await topicDraftRepository.AsUpdateable(entity).UpdateColumns(updateColumns).ExecuteCommandAsync();
            await logService.LogInformationAsync(JsonConvert.SerializeObject(input), nameof(SetVerificationFailureAsync));
            return effectRows > 0;
        }

        public async Task<PagedResultDto<TopicDraftItemDto>> GetMyTopicDraftPagingListAsync(SearchTopicDraftDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            input.PageIndex = input.PageIndex <= 0 ? 1 : input.PageIndex;
            input.PageSize = input.PageSize <= 10 ? 10 : input.PageSize;

            var currentUserId = CurrentUser.Id;
            var authorizationQuery = topicAuthorizationRepository.AsQueryable();
            var query = topicDraftRepository.AsQueryable();
            query = query.Where(a => a.author_id == currentUserId)
                .WhereIF(!string.IsNullOrWhiteSpace(input.TopicName), a => a.name.ToLower().Contains(input.TopicName.Trim().ToLower()));

            if (input.CategoryId != null)
            {
                var children = await categoryService.GetChildrenAsync(input.CategoryId.Value);
                if (children is List<CategoryListItemDto> { Count: > 0 })
                {
                    var categoryIds = children.Select(a => a.id);
                    query = query.Where(t => categoryIds.Contains(t.category_id));
                }
            }

            var totalCount = await query.CountAsync();
            var data = await query.OrderByDescending(t => t.created_time).ToPageListAsync(input.PageIndex, input.PageSize);
            var pagedResult = new PagedResultDto<TopicDraftItemDto>();

            pagedResult.TotalCount = totalCount;
            pagedResult.Data = data.Adapt<List<TopicDraftItemDto>>();

            return pagedResult;
        }

        public async Task<PagedResultDto<TopicListItemDto>> GetMyTopicPagingListAsync(SearchMyTopicDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            input.PageIndex = input.PageIndex <= 0 ? 1 : input.PageIndex;
            input.PageSize = input.PageSize <= 10 ? 10 : input.PageSize;

            var currentUserId = CurrentUser.Id;
            var authorizationQuery = topicAuthorizationRepository.AsQueryable();
            var topicAccessRequestQuery = topicAccessRequestRepository.AsQueryable();

            var query = topicRepository.AsQueryable();
            var queryResults = query.InnerJoin<TopicAuthorizationUserEntity>(authorizationQuery, (a, b) => a.id == b.topic_id)
                .InnerJoin(topicAccessRequestQuery.WhereIF(input.Status != null, a => a.status == input.Status), (a, b, c) => b.topic_id == c.topic_id)
                                .Where((a, b, c) => b.user_id == currentUserId && c.user_id == currentUserId)
                                .WhereIF(!string.IsNullOrWhiteSpace(input.TopicName), a => a.name.ToLower().Contains(input.TopicName.Trim().ToLower()))
                                .SelectMergeTable((a, b, c) => new TopicListItemDto
                                {
                                    id = a.id,
                                    author = a.author,
                                    author_id = a.author_id,
                                    remark = c.remark,
                                    cover = a.cover,
                                    department = a.department,
                                    description = a.description,
                                    name = a.name,
                                    ratings = a.ratings,
                                    status = c.status,
                                    version = a.version,
                                    created_time = c.created_time,
                                });

            var totalCount = await queryResults.CountAsync();
            var data = await queryResults.OrderByDescending(t => t.created_time).ToPageListAsync(input.PageIndex, input.PageSize);
            var pagedResult = new PagedResultDto<TopicListItemDto>();

            pagedResult.TotalCount = totalCount;
            pagedResult.Data = data;

            return pagedResult;
        }

        public async Task<PagedResultDto<TopicDraftItemDto>> GetTopicDraftPagingListAsync(SearchTopicDraftDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            input.PageIndex = input.PageIndex <= 0 ? 1 : input.PageIndex;
            input.PageSize = input.PageSize <= 10 ? 10 : input.PageSize;

            var queryTopic = topicRepository.AsQueryable();
            var query = topicDraftRepository.AsQueryable();
            if (input.CategoryId != null)
            {
                var children = await categoryService.GetChildrenAsync(input.CategoryId.Value);
                if (children is List<CategoryListItemDto> { Count: > 0 })
                {
                    var categoryIds = children.Select(a => a.id);
                    query = query.Where(t => categoryIds.Contains(t.category_id));
                }
            }

            query = query.WhereIF(!string.IsNullOrWhiteSpace(input.TopicName), t => t.name.Contains(input.TopicName));

            var totalCount = await query.CountAsync();
            var data = await query.OrderByDescending(t => t.created_time).ToPageListAsync(input.PageIndex, input.PageSize);
            var pagedResult = new PagedResultDto<TopicDraftItemDto>();
            var releasedTopic = queryTopic.WhereIF(data != null, t => data.Select(a => a.id).Contains(t.id));
            pagedResult.TotalCount = totalCount;
            pagedResult.Data = data.Adapt<List<TopicDraftItemDto>>();

            foreach (var item in pagedResult.Data)
            {
                var releasedItem = releasedTopic.First(t => t.id == item.id);
                if (releasedItem != null)
                {
                    item.is_released = true;
                }
            }

            return pagedResult;
        }

        public async Task<bool> ApproveAsync(BMApproveFlowDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            var topicDraftEntity = await topicDraftRepository.GetByIdAsync(dto.TopicId);
            ArgumentNullException.ThrowIfNull(topicDraftEntity);
            ArgumentNullException.ThrowIfNull(topicDraftEntity.flow_id);

            var processChanged = false;
            HttpResult results = null;
            ProgressStatus nextProgress = ProgressStatus.BusinessModeling;
            var flowDto = new ApproveFlow.Dto.FlowAuditDto
            {
                AuditContent = dto.Remark,
                ActOperate = dto.Action.ToString(),
                FlowInstId = topicDraftEntity.flow_id.Value
            };
            switch (dto.Action)
            {
                case ApproveFlow.Enums.BMFlowAction.Approval:
                    {
                        switch (topicDraftEntity.progress)
                        {
                            case ProgressStatus.ITDeveloping:
                                {
                                    TopicMgrFormCheckService.CheckItDevelopRecordForm(topicDraftEntity);
                                    TopicMgrFormCheckService.CheckOutputParameterForm(topicDraftEntity);
                                    nextProgress = ProgressStatus.Validation;
                                    processChanged = true;
                                }
                                break;
                            case ProgressStatus.Validation:
                                {
                                    TopicMgrFormCheckService.CheckVerifyForm(topicDraftEntity);
                                    nextProgress = ProgressStatus.Publish;
                                    processChanged = true;
                                }
                                break;
                            case ProgressStatus.Publish:
                                {
                                    TopicMgrFormCheckService.CheckIsPublishedForm(topicDraftEntity);
                                    nextProgress = ProgressStatus.Completed;
                                    processChanged = true;
                                }
                                break;
                            default:
                                break;
                        }

                        results = await flowApplyProxyService.SendApproveAsync(flowDto);

                    }
                    break;
                case ApproveFlow.Enums.BMFlowAction.Reject:
                    {

                        switch (topicDraftEntity.progress)
                        {
                            case ProgressStatus.ITDeveloping:
                                nextProgress = ProgressStatus.BusinessModeling;
                                processChanged = true;
                                break;
                            case ProgressStatus.Validation:
                                nextProgress = ProgressStatus.ITDeveloping;
                                processChanged = true;
                                break;
                            case ProgressStatus.Publish:
                                nextProgress = ProgressStatus.Validation;
                                processChanged = true;
                                break;
                            default:
                                break;
                        }

                        results = await flowApplyProxyService.SendRejectAsync(flowDto);
                    }
                    break;
                case ApproveFlow.Enums.BMFlowAction.RejectEnd:
                    {
                        nextProgress = ProgressStatus.Completed;
                        processChanged = true;
                        results = await flowApplyProxyService.SendRejectEndAsync(flowDto);

                    }
                    break;
                default:
                    break;
            }

            ArgumentNullException.ThrowIfNull(results);

            if (!results.Success) throw Oops.Oh(results.Msg);
            if (processChanged && results.Success)
            {
                topicDraftEntity.progress = nextProgress;
                await topicDraftRepository.UpdateAsync(topicDraftEntity);
            }
            return results.Success;
        }
    }
}
