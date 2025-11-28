using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataCatalog.Dtos;
using DataAssetManager.DataApiServer.Application.Services;
using DataAssetManager.DataTableServer.Application;

using Furion.DatabaseAccessor;
using Furion.EventBus;
using Furion.JsonSerialization;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Extensions;
using ITPortal.Core.Services;
using ITPortal.Core.SqlParse;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using Swashbuckle.AspNetCore.Swagger;

using System.Collections.Generic;
using System.Data;
using System.Security.Policy;
using System.Text;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataApiService : BaseService<DataApiEntity, DataApiQueryDto, string>, IDataApiService, ITransient
    {
        private readonly ISwaggerProvider _swaggerProvider;
        //private readonly IMapper _mapper;
        //private readonly SwaggerDynamicConfigService _swaggerDynamicConfigService;
        private readonly IDataCatalogService _dataCatalogService;
        private readonly IDataSourceService _sourceService;
        private readonly IDataTableService _tableService;
        private readonly IDataColumnService _columnService;
        private readonly CacheDbQueryFactory _cacheDbQueryFactory;
        private readonly ILogger<DataApiService> _logger;
        private readonly IEventPublisher _eventPublisher;
        public DataApiService(ISqlSugarClient db, IDistributedCacheService cache,
            ISwaggerProvider swaggerProvider,
            //SwaggerDynamicConfigService swaggerDynamicConfigService,
            ILogger<DataApiService> logger,
            IDataSourceService sourceService,
            IDataTableService tableService,
             IDataColumnService dataColumnService,
             IDataCatalogService dataCatalogService,
             CacheDbQueryFactory cacheDbQueryFactory,
             IEventPublisher eventPublisher) : base(db, cache,false,true)
        {
            _swaggerProvider = swaggerProvider;
            //_swaggerDynamicConfigService = swaggerDynamicConfigService;
            _sourceService = sourceService;
            _tableService = tableService;
            _columnService = dataColumnService;
            _cacheDbQueryFactory = cacheDbQueryFactory;
            _dataCatalogService = dataCatalogService;
            //_mapper = mapper;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<List<RouteInfo>> All()
        {
            //查询表的所有
            var list = await CurrentDb.Queryable<DataApiEntity>().Where(f => f.Status == 2).ToListAsync();
            //.LeftJoin<DataCatalogEntity>((f,dc)=>f.SourceId=).ToListAsync();
            return list.Select(f => new RouteInfo()
            {
                ApiName = f.ApiName,
                ApiUrl = f.ApiUrl,
                ApiVersion = f.ApiVersion,
                Deny = f.Deny,
                ExecuteConfig = f.ExecuteConfig,
                Id = f.Id,
                RateLimit = f.RateLimit,
                Remark = f.Remark,
                ReqMethod = f.ReqMethod,
                ReqParams = f.ReqParams,
                ResParams = f.ResParams,
                ResType = f.ResType,
                SourceId = f.SourceId,
                TableId = f.TableId,
                OwnerDepart =f.OwnerDepart,
                Status = f.Status,
            }).ToList();
            //return _mapper.Map<List<RouteInfo>>(list);
        }

        public async Task<List<RouteInfo>> AllFromCache(bool clearCache = false)
        {
            //if (clearCache) await _cache.RemoveAsync(DataAssetManagerConst.RouteRedisListKey);
            return await _cache.GetObjectAsync(DataAssetManagerConst.RouteRedisListKey, All, null, clearCache);
        }

        //public async Task<List<RouteInfo>> InitRedisHash()
        //{
        //    var list = await AllFromCache();
        //    foreach (var item in list)
        //    {
        //        _cache.HashSet(DataAssetManagerConst.RouteRedisKey, item.ApiServiceUrl, item);
        //    }
        //    return list;
        //}

        public async Task<string> InitRoutes(bool clearCache = false)
        {
            //if (clearCache)
            //{
            //    await _cache.RemoveAsync(DataAssetManagerConst.RouteRedisKey);
            //    await _cache.RemoveAsync(DataAssetManagerConst.DataApis_HashKey);
            //}
            var list = await CurrentDb.Queryable<DataApiEntity>().Where(f => f.Status == 2).ToListAsync();
            if (this._cache.CacheType == CacheType.Memory)
            {
                foreach (var item in list)
                {
                    //await _cache.HashRemoveAsync(DataAssetManagerConst.RouteRedisKey, item.ApiServiceUrl);
                    var result2 = await this._cache.HashSetAsync(DataAssetManagerConst.RouteRedisKey, item.ApiServiceUrl.ToLower(), item.Adapt<RouteInfo>());
                    //await _cache.HashRemoveAsync(DataAssetManagerConst.RouteRedisKey, item.Id);
                    result2 = await this._cache.HashSetAsync(DataAssetManagerConst.DataApis_HashKey, item.Id, item.Adapt<RouteInfo>());
                }
            }
            else
            {
                foreach (var item in list)
                {
                    var json = JSON.Serialize(item);
                    //await _cache.HashRemoveAsync(DataAssetManagerConst.RouteRedisKey, item.ApiServiceUrl);
                    var result2 = await this._cache.HashSetAsync(DataAssetManagerConst.RouteRedisKey, item.ApiServiceUrl.ToLower(), json);
                    //await _cache.HashRemoveAsync(DataAssetManagerConst.RouteRedisKey, item.Id);
                    result2 = await this._cache.HashSetAsync(DataAssetManagerConst.DataApis_HashKey, item.Id, json);
                }
            }
            _swaggerProvider.GetSwagger("services");
            return "ok";
        }


        public async Task<int> Count()
        {
            return await CurrentDb.Queryable<DataApiEntity>().CountAsync();
        }

        public async Task<int> CountFromCache()
        {
            return await _cache.GetIntAsync(DataAssetManagerConst.DataApis_Count, async () =>
            {
                return await CurrentDb.Queryable<DataApiEntity>().CountAsync();
            }, TimeSpan.FromSeconds(60)) ?? 0;
        }

        public override ISugarQueryable<DataApiEntity> BuildFilterQuery(DataApiQueryDto filter)
        {
            filter.ApiName = filter.ApiName?.Trim()?.ToLower();
            filter.ConfigType = filter.ConfigType?.Trim()?.ToLower();
            var query = CurrentDb.Queryable<DataApiEntity>()
                  .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), f => f.Id == filter.Id)
                  .WhereIF(!string.IsNullOrWhiteSpace(filter.ApiName), f => SqlFunc.ToLower(f.ApiName).Contains(filter.ApiName) || f.ApiUrl.Contains(filter.ApiName))
                  .WhereIF(!string.IsNullOrWhiteSpace(filter.ApiUrl), f => SqlFunc.ToLower(f.ApiUrl).Contains(filter.ApiUrl))
                  .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                  .WhereIF(!string.IsNullOrWhiteSpace(filter.ConfigType), f => SqlFunc.JsonField(f.ExecuteConfig, "configType") == filter.ConfigType)
                  .OrderByIF(filter.OrderField.IsNotNullOrWhiteSpace(), filter.OrderField);
            if (!filter.OrderField.IsNotNullOrWhiteSpace())
                query = query.OrderByDescending(f => f.CreateTime);
            return query;
        }

        private const string DEF_VERSION = "v1.0.0";
        /// <summary>
        /// 保存或修改数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        [Furion.DatabaseAccessor.UnitOfWork()]
        public async Task<DataApiEntity> Save(DataApiCreateDto entity, bool clearCache = true)
        {
            DataApiEntity data = null;
            try
            {
                var model = await ShareCode(entity);
                if (entity.Id.IsNullOrWhiteSpace())
                    data = await BuildFilterQuery(new DataApiQueryDto() { ApiUrl = entity.ApiUrl, ApiVersion = entity.ApiVersion }).FirstAsync();
                else
                    data = await BuildFilterQuery(new DataApiQueryDto() { Id = entity.Id, ApiVersion = entity.ApiVersion }).FirstAsync();
                if (data == null && string.IsNullOrWhiteSpace(model.Id))
                {
                    //model.Id = entity.Id = Guid.NewGuid().ToString();
                    await Create(model, clearCache);
                    model.Id = entity.Id;
                    return model;
                }
                model.Id = data.Id;
                await Modify(model, clearCache);
                return model;
            }
            finally
            {
                //await _eventPublisher.PublishAsync(new ITPortal.Core.LightElasticSearch.RedisEventSource(DataAssetManagerConst.DataApis_HashKey));
                await _eventPublisher.PublishAsync(new ITPortal.Core.LightElasticSearch.RedisEventSource(DataAssetManagerConst.DataTable_UserHashKey));
            }
        }

        public override async Task<DataApiEntity> Single(DataApiQueryDto filter)
        {
            return await BuildFilterQuery(filter).FirstAsync(f => f.Status == 2);
        }



        /// <summary>
        /// 注册数据资产API
        /// </summary>
        /// <param name="apiConfig"></param>
        /// <returns></returns>
        public void Register(RouteInfo apiConfig)
        {
            var result = _cache.HashSet(DataAssetManagerConst.RouteRedisKey, apiConfig.ApiServiceUrl.ToLower(), apiConfig);
            result = _cache.HashSet(DataAssetManagerConst.DataApis_HashKey, apiConfig.Id, apiConfig);

            // 手动触发 Swagger 文档的重新生成
            _swaggerProvider.GetSwagger("services");

            //string routeName = apiConfig.ApiUrl.Split('/')[0];
            //_swaggerDynamicConfigService.AddSwaggerDoc(routeName, new OpenApiInfo()
            //{
            //    Title = apiConfig.ApiName,
            //    Version = apiConfig.ApiVersion,
            //    Description = apiConfig.Remark
            //});
            //_swaggerDynamicConfigService.AddSwaggerEndpoint(apiConfig.ApiName, $"/swagger/{routeName}/swagger.json");
            //_swaggerProvider.GetSwagger(routeName); 
        }


        /// <summary>
        /// 注册数据资产API
        /// </summary>
        /// <param name="apiConfig"></param>
        /// <returns></returns>
        public async Task<dynamic> Release(RouteInfo apiConfig)
        {
            var data = await this.Get(apiConfig.Id);
            if (data == null) return "The data asset API does not exist";
            data.Status = ApiState.RELEASE.ToInt();
            var result = await Modify(data);
            Register(data.Adapt<RouteInfo>());//_mapper.Map<DataApiEntity, RouteInfo>(data)
            return "OK";
        }


        public async void Register(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new AppFriendlyException($"{id}The data asset API does not exist", "500");
            var apiInfo = await Get(id);// this._cache.HashGet<RouteInfo>(DataAssetManagerConst.DataApis_HashKey, id);
            Register(apiInfo.Adapt<RouteInfo>());//_mapper.Map<DataApiEntity, RouteInfo>(apiInfo));
        }

        /// <summary>
        /// 注销数据资产API
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        public async Task Unregister(string apiUrl)
        {
            if (string.IsNullOrWhiteSpace(apiUrl)) throw new AppFriendlyException($"{apiUrl}The data asset API does not exist", "500");
            var apiInfo = this._cache.HashGet<RouteInfo>(DataAssetManagerConst.RouteRedisKey, apiUrl.ToLower());
            await Unregister(apiInfo.Id);
        }


        public async Task UnregisterById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new AppFriendlyException($"{id}The data asset API does not exist", "500");
            var data = await Get(id);// this._cache.HashGet<RouteInfo>(DataAssetManagerConst.DataApis_HashKey, id);
            if (data != null) await Unregister(data.Adapt<RouteInfo>());// _mapper.Map<DataApiEntity, RouteInfo>(data));
        }

        /// <summary>
        /// 注销数据资产API
        /// </summary>
        /// <param name="apiInfo"></param>
        /// <returns></returns>
        public async Task Unregister(RouteInfo apiInfo)
        {
            var result = await _cache.HashRemoveAsync(DataAssetManagerConst.RouteRedisKey, apiInfo.ApiServiceUrl.ToLower());
            result = await _cache.HashRemoveAsync(DataAssetManagerConst.DataApis_HashKey, apiInfo.Id);

            // 手动触发 Swagger 文档的重新生成
            _swaggerProvider.GetSwagger("services");
        }

        /// <summary>
        /// 注销数据资产API
        /// </summary>
        /// <param name="apiInfo"></param>
        /// <returns></returns>
        public async Task<dynamic> Cancel(RouteInfo apiInfo)
        {
            var data = await this.Get(apiInfo.Id);
            if (data == null) return "The data asset API does not exist";
            data.Status = ApiState.CANCEL.ToInt();
            await Modify(data);

            await Unregister(data.Adapt<RouteInfo>());// _mapper_mapper.Map<DataApiEntity, RouteInfo>(data));
            return "Ok()";
        }

        public async Task<DataApiEntity> Copy(string id)
        {
            var data = await Get(id);
            if (data == null)
                throw new AppFriendlyException("The data asset API does not exist！", "404");
            var copyData = data.Adapt<DataApiEntity>();// _mapper_mapper.Map(data, new DataApiEntity());
            //copyData.Id = Guid.NewGuid().ToString().Replace("-", "");
            copyData.ApiName = $"{data.ApiName}_Copy_{DateTimeOffset.Now.ToString("yyyyMMddHHmmss")}";
            copyData.ApiUrl = $"{data.ApiUrl}_copy";
            copyData.Status = ApiState.WAIT.ToInt();
            var result = await Create(copyData);
            if (result > 0) return copyData;
            throw new AppFriendlyException("Save failed！", "500");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<dynamic> GetDataApiDetailById(string id)
        {
            ApiHeader apiHeader = new ApiHeader();
            try
            {
                apiHeader.ApiKey = ITPortal.Core.DESEncryption.Encrypt(id);
                apiHeader.SecretKey = ITPortal.Core.DESEncryption.Encrypt(CurrentUser?.UserId ?? "1");
            }
            catch (Exception e)
            {
                _logger.LogError($"Api{id}'s ApiKey or SecretKey encode error:{e.Message},\r\n{e.StackTrace}");
            }
            var dataApiEntity = await Get(id);
            return new { data = dataApiEntity.Adapt<DataApiDto>(), header = apiHeader };// _mapper.Map<DataApiEntity, DataApiDto>(dataApiEntity)
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public async Task<DataApiEntity> GetBySourceId(string sourceId)
        {
            return await CurrentDb.Queryable<DataApiEntity>().FirstAsync(f => SqlFunc.JsonField(f.ExecuteConfig, "source_id") == sourceId);
        }

        public async Task<Result> DeleteByUrlList(List<string> urlList)
        {
            var list = await AllFromCache();
            var result = await Delete(list.Where(f => urlList.Contains(f.ApiUrl.ToLower().Replace(" ", "").Replace("//", "/"))).Select(f => f.Id).ToArray(),false);
            return new Result().SetError(result);
        }

        /// <summary>
        /// 获取当前map主题但是没有生成api的所有表清单
        /// </summary>
        /// <returns></returns>
        public async Task<List<DataTableEntity>> GetNoApiTables()
        {
            var catalogQuery = CurrentDb.Queryable<DataCatalogEntity>().Where(f => f.Status == 1);
            var query = catalogQuery
                  .InnerJoin(CurrentDb.Queryable<DataCatalogTableMapping>(), (f, ctm) => f.Id == ctm.CatalogId)
                  .InnerJoin(CurrentDb.Queryable<DataTableEntity>().Where(t => t.Status == 1), (f, ctm, t) => ctm.TableId == t.Id)
                  .InnerJoin(CurrentDb.Queryable<DataSourceEntity>(), (f, ctm, t, s) => t.SourceId == s.Id)
                  .LeftJoin(CurrentDb.Queryable<DataApiEntity>(), (f, ctm, t, s,a) => t.Id == a.TableId)
                  .Where((f, ctm, t,s, a) => a.Id == null)
                  .OrderBy((f, ctm, t, s, a) =>f.Code)
                  .Select((f, ctm, t, s, a) =>
                    //new APIAutoGenParam
                    //{
                    //    ApiName = $"{topicName}{mappingDto.TableName}",
                    //    ApiUrl = $"/{mappingDto.SourceName.Replace("_", "/")}/{mappingDto.TableName}",
                    //    TableId = mappingDto.Id,
                    //    Remark = $"生成方式：系统自动生成\n" +
                    //    $"主题业务含义：{one.Remark}\n" +
                    //              $"主题英文：{one.Code}\n" +
                    //              $"数据资产主题域：{topicName}{mappingDto.TableName}"
                    //})
                    new DataTableEntity()
                    {
                        Id = t.Id,
                        TableName = t.TableName,
                        Alias = t.Alias,
                        TableComment = t.TableComment,
                        Remark = t.Remark,
                        SourceId = t.SourceId,
                        SourceName = s.SourceName,
                        CtlCode = f.Code,
                        CtlId = f.Id,
                        CtlName = f.Name,
                        CtlRemark = f.Remark,
                        CreateBy = t.CreateBy,
                        CreateTime = t.CreateTime,
                        OwnerDepart = t.OwnerDepart,
                        JsonSqlConfig = t.JsonSqlConfig,
                        Status = t.Status,
                        UpdateFrequency = t.UpdateFrequency,
                        UpdateMethod = t.UpdateMethod,
                        Reviewable = t.Reviewable,
                        DataTimeRange = t.DataTimeRange
                    })
                  .Distinct();
            return await query.ToListAsync();
        }



        [UnitOfWork(true)]
        public async Task<IResult<List<DataApiEntity>>> CreateMappTableApi(List<DataTableEntity> tableList)
        {
            var result = new Result<List<DataApiEntity>>();
            if (tableList?.Count <= 0) return result;
            List<DataCatalogEntity> all = null;
            var prevCtlId = string.Empty;
            List<APIAutoGenParam> paramsList = new List<APIAutoGenParam>();

            foreach (var table in tableList)
            {
                try
                {
                    if (!prevCtlId.Equals(table.CtlId))
                        all = await _dataCatalogService.GetParentTopic(new[] { table.CtlId });

                    DataCatalogEntity one = all.FirstOrDefault(f => f.Id == table.CtlId);
                    if (one == null)
                    {
                        result.SetError($"{table.CtlId} Topic disabling has not taken effect and cannot be configured.");
                        _logger.LogError($"{table.CtlId} Topic disabling has not taken effect and cannot be configured.");
                        continue;
                        //throw new AppFriendlyException($"{table.CtlId} Topic disabling has not taken effect and cannot be configured.", 5404);
                    }


                    var topicName = new StringBuilder();
                    for (int i = 1; i < all.Count; i++)
                    {
                        topicName.Append(all[i].Name).Append("-");
                    }

                    var param = new APIAutoGenParam
                    {
                        ApiName = $"{topicName}{table.TableName}",
                        ApiUrl = $"/{table.SourceName.Replace("_", "/")}/{table.TableName}",
                        TableId = table.Id,
                        Remark = $" 生成方式：系统自动生成 \r\n " +
                                    $" 主题业务含义：{one.Remark}\r\n" +
                                    $" 主题英文：{one.Code}\r\n" +
                                    $" 数据资产主题域：{topicName}{table.TableName}"
                    };
                    paramsList.Add(param);
                }
                catch (Exception e)
                {
                    result.AddError($"Build data Error:{e.Message}");
                    _logger.LogError("Global Exception!ex={0}, StackTrace={1}", e.Message, e.StackTrace);
                }
            }

            if (paramsList.Count <= 0) return result;
            try
            {
                var b = await AutoCreate(paramsList);
                result.AddMsg(b);
                _logger.LogInformation("自动创建线程:{0}.结果：{1}", Environment.CurrentManagedThreadId, b.Success);
            }
            catch (Exception e)
            {
                result.AddError($"Auto Create Error:{e.Message}");
                _logger.LogError("Global Exception Auto Create Error:!ex={0}, StackTrace={1}", e.Message, e.StackTrace);
            }
            return result;
        }



        [Furion.DatabaseAccessor.UnitOfWork()]
        public async Task<IResult<List<DataApiEntity>>> AutoCreate(List<APIAutoGenParam> paramsList)
        {
            var result = new Result<List<DataApiEntity>>();
            if (paramsList == null || paramsList.Count == 0)
                return result.SetError("no data to create");

            var list = new List<DataApiEntity>();
            try
            {
                _logger.LogInformation("------------ API 自动生成开始 ------------");
                using (var uow = CurrentDb.CreateContext())
                {
                    foreach (var param in paramsList)
                    {
                        try
                        {
                            var bResult = await AutoBuildApi(param);
                            if (bResult.Success)
                            {
                                list.AddRange(bResult.Data);
                            }
                            else
                            {
                                if (result.Data == null) result.Data = bResult.Data;
                                else result.Data.AddRange(bResult.Data);
                                result.AddMsg(bResult);
                            }
                        }
                        catch (Exception ex)
                        {
                            result.AddError($"API 自动生成失败: {ex.Message}, param: {JSON.Serialize(param)}");
                            _logger.LogError($"API 自动生成失败: {ex.Message}, param: {JSON.Serialize(param)}");
                        }
                    }
                    uow.Commit();
                }
            }
            finally
            {
                await _eventPublisher.PublishAsync(new ITPortal.Core.LightElasticSearch.RedisEventSource(DataAssetManagerConst.DataApis_HashKey));
                //var _ = Task.Run(async () =>
                //{
                 
                //    //await _eventPublisher.PublishAsync(new ITPortal.Core.LightElasticSearch.RedisEventSource(DataAssetManagerConst.DataTable_HashKey));
                //});
            }
            _logger.LogInformation($"API 自动生成数量: {list.Count}");
            _logger.LogInformation("------------ API 自动生成结束 ------------");
            return result;
        }

        [Furion.DatabaseAccessor.UnitOfWork()]
        public async Task<Result<List<DataApiEntity>>> AutoBuildApi(APIAutoGenParam param)
        {
            var result = new Result<List<DataApiEntity>>();
            using (var uow = CurrentDb.CreateContext())
            {
                // 根据表id获取表信息
                var table = await _tableService.Get(param.TableId);
                // 获取数据源信息
                var dataSource = await _sourceService.Get(table.SourceId);
                // 获取表列信息
                var columns = await _columnService.GetByTableId(param.TableId);
                var sourceId = dataSource.Id;

                // 获取表预览数据
                var db = _cacheDbQueryFactory.CreateSqlClient(dataSource.DbSchema);
                ISqlParser parser = SqlParserFactory.CreateParser(dataSource.DbType);
                var select = parser.BuildSelectFromTable(db, table.TableName);
                _logger.LogInformation("Data Preview SQL: {0}", select.SqlBuilder.SqlQueryBuilder.ToSqlString());
                var data = await select.FirstAsync();

                IDictionary<string, object> dataViewMap = new Dictionary<string, object>();
                if (data != null) dataViewMap = (IDictionary<string, object>)data;

                // 创建 JSON To SQL API  sqlapi
                var configType = ConfigType.SQL; // TODO 暂时先改回 SQL 模式的类型
                var suffix = $"/{configType.ToString().ToLower()}{"Query"}";
                var apiName = $"{param.ApiName}_{configType.ToString().ToLower()}_model";

                var jsonApiDto = new DataApiCreateDto
                {
                    ApiName = apiName,
                    Remark = param.Remark,
                    SourceId = sourceId,
                    TableId = table.Id,
                    OwnerDepart = table.OwnerDepart,
                    Status = ApiState.RELEASE.ToInt().ToString(),
                    ApiUrl = param.ApiUrl.Replace(" ", "").Replace("//", "/") + suffix,
                    ReqMethod = "POST",
                    ResType = "JSON",
                    ExecuteConfig = new ExecuteConfig
                    {
                        sourceId = sourceId,
                        tableId = param.TableId,
                        tableName = table.TableName,
                        configType = configType.ToInt().ToString()
                    }
                };

                if (columns == null || !columns.Any())
                {
                    result.AddError(string.Format("Unable to build table Bootstrap mode API failed to obtain table column information，table id:{0}, table name:{1}, API URL: {2}", param.TableId, table.TableName, jsonApiDto.ApiUrl));
                    _logger.LogError("Unable to build table Bootstrap mode API failed to obtain table column information，table id:{0}, table name:{1}, API URL: {2}", param.TableId, table.TableName, jsonApiDto.ApiUrl);
                    throw new DataException("Unable to build table boot mode API, failed to obtain table column information");
                }

                var dto = new DataApiCreateDto
                {
                    ApiName = $"{param.ApiName}_table_bootstrap_model",
                    Remark = param.Remark,
                    SourceId = sourceId,
                    TableId = table.Id,
                    OwnerDepart = table.OwnerDepart,
                    ApiUrl = param.ApiUrl.Replace(" ", "").Replace("//", "/") + "/query",
                    ReqMethod = "GET",
                    ResType = "JSON",
                    Status = ApiState.RELEASE.ToInt().ToString(),
                    ExecuteConfig = new ExecuteConfig
                    {
                        sourceId = sourceId,
                        tableId = param.TableId,
                        tableName = table.TableName,
                        configType = ConfigType.FORM.ToInt().ToString()
                    }
                };

                var configFieldParams = new List<FieldParam>();
                var reqParams = new List<ReqParam>();
                var resParams = new List<ResParam>();

                try
                {
                    foreach (var column in columns)
                    {
                        var fieldParam = new FieldParam
                        {
                            columnName = column.ColName,
                            columnComment = column.ColComment,
                            dataType = column.DataType,
                            dataDefault = column.DataDefault,
                            dataPrecision = string.IsNullOrEmpty(column.DataPrecision) ? (int?)null : int.Parse(column.DataPrecision),
                            dataLength = string.IsNullOrEmpty(column.DataLength) ? (int?)null : int.Parse(column.DataLength),
                            dataScale = string.IsNullOrEmpty(column.DataScale) ? (int?)null : int.Parse(column.DataScale),
                            columnPosition = column.ColPosition,
                            columnKey = column.ColKey,
                            columnNullable = column.Nullable,
                            reqable = "1",
                            resable = "1"
                        };
                        configFieldParams.Add(fieldParam);
                    }
                }
                catch (Exception ex)
                {
                    result.AddError($"Configuration field information is abnormal ERROR: {ex.Message}");
                    _logger.LogError(ex, $"Configuration field information is abnormal ERROR: {ex.Message}");
                    throw new DataException($"Configuration field information is abnormal：{ex.Message}");
                }

                try
                {
                    // 配置字段信息
                    var betweenConditionList = new List<string> { "DATE", "TIME", "DATETIME", "DATETIMEOFFSET", "TIMESTAMP", "DATETIME2" };
                    foreach (var fieldParam in configFieldParams)
                    {
                        object exampleValue = null;
                        string dataValue = null;
                        if (dataViewMap.TryGetValue(fieldParam.columnName, out exampleValue))
                            dataValue = exampleValue?.ToString();
                        if (fieldParam.reqable == "1")
                        {
                            var reqParam = new ReqParam
                            {
                                paramName = fieldParam.columnName,
                                nullable = fieldParam.columnNullable,
                                paramComment = fieldParam.columnComment,
                                paramType = fieldParam.dataType,
                                whereType = betweenConditionList.Contains(fieldParam.dataType.ToUpper()) ? "between" : "eq",
                                exampleValue = dataValue ?? "ExampleValue",
                                defaultValue = dataValue ?? "DefaultValue"
                            };
                            reqParams.Add(reqParam);
                        }
                        if (fieldParam.resable == "1")
                        {
                            var resParam = new ResParam
                            {
                                fieldName = fieldParam.columnName,
                                fieldComment = fieldParam.columnComment,
                                dataType = fieldParam.dataType,
                                exampleValue = dataValue ?? "ExampleValue",
                                fieldAliasName = fieldParam.columnName
                            };
                            resParams.Add(resParam);
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.AddError($"Configuration field information exception [response field and request field] ERROR: {ex.Message}");
                    _logger.LogError(ex, $"Configuration field information exception [response field and request field] ERROR: {ex.Message}");
                    throw new DataException($"Configuration field information exception [response field and request field]：{ex.Message}");
                }

                dto.ExecuteConfig.fieldParams = configFieldParams;
                dto.ReqParams = reqParams;
                dto.ResParams = resParams;

                DataApiEntity dataApi;
                DataApiEntity jsonApi;
                try
                {
                    dataApi = await Save(dto, false);
                    jsonApi = await Save(jsonApiDto, false);
                }
                catch (Exception ex)
                {
                    result.AddError($"AutoBuildApi Save ERROR: {ex.Message}");
                    _logger.LogError(ex, $"AutoBuildApi Save ERROR: {ex.Message}");
                    throw new DataException($"Save error：{ex.Message},\r\n{ex.StackTrace}");
                }

                // 发布 API
                await Release(new RouteInfo() { Id = jsonApi.Id });
                await Release(new RouteInfo() { Id = dataApi.Id });

                uow.Commit();

                result.Data = new List<DataApiEntity> { jsonApi, dataApi };
            }
            return result;
        }

        private async Task<DataApiEntity> ShareCode(DataApiCreateDto entity)
        {
            if (entity == null) throw new ArgumentNullException("The ShareCode parameter of the api cannot be empty");
            entity.SourceId = entity.ExecuteConfig.sourceId;
            var dataSource = await _sourceService.Get(entity.ExecuteConfig.sourceId);
            if (dataSource == null) throw new ArgumentNullException($"api的DataSourceid:{entity.ExecuteConfig.sourceId},Data does not exist！");
            var tableEntity = await _tableService.Get(entity.ExecuteConfig.tableId);
            entity.TableId = tableEntity.Id;
            entity.OwnerDepart = tableEntity.OwnerDepart;
            var configType = entity.ExecuteConfig.configType.GetEnum<ConfigType>();
            // 如果 API URL 未定义, 自动生成API路径
            if (entity.ApiUrl.IsNullOrWhiteSpace())
            {
                entity.ApiUrl = $"/{dataSource.SourceName.Replace("_", "/")}/{tableEntity.TableName}/{(ConfigType.FORM == configType ? "" : configType.ToString().ToLower())}Query";
            }

            if (entity.ApiVersion.IsNullOrWhiteSpace()) entity.ApiVersion = DEF_VERSION;
            else entity.ApiVersion = DEF_VERSION;//版本升级规则
            entity.ExecuteConfig.tableName = tableEntity.TableName;
            if (!entity.ExecuteConfig.pageSizeLimit.HasValue && tableEntity.JsonSqlConfig.Limit.HasValue)
                entity.ExecuteConfig.pageSizeLimit = tableEntity.JsonSqlConfig.Limit.Value;

            //检查不能低于限流配置: 必须有限流参数，若界面配置无输入，则取默认值(配置中设置)
            if (entity.RateLimit == null)
            {
                entity.RateLimit = new RateLimit() { seconds = 60, times = 3 };
                if (entity.Seconds.HasValue)
                    entity.RateLimit.seconds = entity.Seconds.Value;
                if (entity.Times.HasValue)
                    entity.RateLimit.times = entity.Times.Value;
                if (entity.Enable == "1" || entity.Enable == "0")
                    entity.RateLimit.enable = entity.Enable;
            }

            if (ConfigType.SQL == configType || ConfigType.JSON == configType)
            {
                var reqMethod = entity.ReqMethod;
                if (!entity.ReqMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new DataException("SQL OR JSON Type Only supports POST requests !");
                }
                entity.ExecuteConfig.sqlText = $"SELECT * FROM {tableEntity.TableName}";
            }
            else if (ConfigType.FORM == configType)
            {
                if (entity.ReqParams == null || entity.ReqParams.Count == 0)
                {
                    throw new DataException("ReqParams Cannot be empty !");
                }
                entity.ReqParams.ForEach(param =>
                {
                    var whereType = param.whereType.GetWereTypeInfo();
                    if (whereType.Key != "0") param.whereType = whereType.KeyName;
                });
                var cloumns = entity.ResParams.Select(f => f.fieldName).ToArray();
                ISqlParser sqlparser = SqlParserFactory.CreateParser(dataSource.DbSchema.Dbtype);
                var where = sqlparser.BuildConditions(entity.ReqParams);
                entity.ExecuteConfig.sqlText = sqlparser.BuildSelectFromTableToSql(CurrentDb, tableEntity.TableName, cloumns)
                    .Append(where).ToString();
                _logger.LogDebug("gen sql:" + entity.ExecuteConfig.sqlText);
            }
            return entity.Adapt<DataApiEntity>();
        }

        public async Task<Dictionary<string, HashSet<string>>> GetTopTopicApiList(string[] ctlIds = null, string searchkey = "")
        {
            Func<Task<Dictionary<string, HashSet<string>>>> getData = async () =>
            {
                var list = new List<TreeEntity>();
                if ((ctlIds == null || ctlIds.Length == 0) && string.IsNullOrWhiteSpace(searchkey))
                    list = await _dataCatalogService.GetTreeTopicFromCache();
                list = await _dataCatalogService.GetTreeTopic(ctlIds, searchkey);

                var tables = await _tableService.AllFromCache();
                //var catalogTables = tables.GroupBy(f => f.CtlId).Select(d => new { d.Key, count = d.Count() });
                var apiList = await AllFromCache();// _cache.GetObjectAsync<List<RouteInfo>>(DataAssetManagerConst.RouteRedisListKey)??new List<RouteInfo>();
                var resultDict = new Dictionary<string, HashSet<string>>();
                list.ForEach(f =>
                {
                    var key = f.Code.Replace(" ", "").Replace("-", "_");
                    resultDict.TryAdd(key, new HashSet<string>());
                    var apis = GetChildApis(f, f.Children, tables);
                    resultDict[key] = apiList.Where(f => apis.Contains(f.TableId)).Select(f => f.ApiServiceUrl).ToHashSet<string>();
                });
                return resultDict;
            };

            var cacheKey = $"{DataAssetManagerConst.RedisKey}DataCatalog:TopTopicApi";
            if (!searchkey.IsNullOrWhiteSpace()) cacheKey = cacheKey + searchkey;
            if (ctlIds != null && ctlIds.Count() > 0) cacheKey = cacheKey + string.Join("_", ctlIds);

            if (cacheKey.Length > 64) return await getData();
            else return await _cache.GetObjectAsync(cacheKey, getData, TimeSpan.FromSeconds(60));
        }

        private List<string> GetChildApis(TreeEntity pentity, List<TreeEntity> childrens, List<DataTableInfo> tables)
        {
            var list = new List<string>();
            foreach (TreeEntity child in childrens)
            {
                list.AddRange(tables.Where(d => child.Key == d.CtlId).Select(f => f.Id));
                list.AddRange(GetChildApis(child, child.Children, tables));
            }
            list.AddRange(tables.Where(d => pentity.Key == d.CtlId).Select(f => f.Id));
            return list;
        }

        //public override async Task RefreshCache()
        //{
        //    await _cache.RemoveAsync(DataAssetManagerConst.RouteRedisKey);
        //    await _cache.RemoveAsync(DataAssetManagerConst.DataApis_HashKey);
        //    await base.RefreshCache();
        //}
    }
}