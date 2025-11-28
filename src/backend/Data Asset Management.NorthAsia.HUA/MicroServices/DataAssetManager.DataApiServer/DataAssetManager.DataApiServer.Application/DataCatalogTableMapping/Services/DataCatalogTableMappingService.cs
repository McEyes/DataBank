using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataCatalog.Dtos;
using DataAssetManager.DataTableServer.Application;

using Furion.DatabaseAccessor;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using Microsoft.Extensions.Logging;

using StackExchange.Profiling.Internal;

using System.Text;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataCatalogTableMappingService : IDataCatalogTableMappingService, ITransient
    {
        /// <summary>
        /// 当前用户信息
        /// </summary>
        public UserInfo CurrentUser { get; protected set; }

        public ISqlSugarClient CurrentDb { get; protected set; }
        protected IDistributedCacheService _cache { get; private set; }
        private readonly ILogger<DataCatalogTableMappingService> _logger;
        private readonly IDataTableService _dataTableService;
        private readonly IDataCatalogService _dataCatalogService;
        private readonly IDataApiService _apiService;

        public DataCatalogTableMappingService(ISqlSugarClient db, IDistributedCacheService cache, ILogger<DataCatalogTableMappingService> logger,
            IDataTableService dataTableService, IDataCatalogService dataCatalogService, IDataApiService dataApiService)
        {
            CurrentDb = db;
            this._cache = cache;
            CurrentUser = App.HttpContext.GetCurrUserInfo() ?? new UserInfo();
            _logger = logger;
            _dataTableService = dataTableService;
            _dataCatalogService = dataCatalogService;
            _apiService = dataApiService;
        }

        [UnitOfWork(true)]
        public async Task<bool> SaveMapping(CatalogMappingDto data)
        {
            string ctlId = data.CtlId;
            if (ctlId.IsNullOrWhiteSpace()) throw new Exception("参数不能为空！");
            if (data.MetadataTableDtoList == null) data.MetadataTableDtoList = new List<DataTableEntity>();

            using (var uow = CurrentDb.CreateContext())
            {
                // 获取该主题域下面的所有表
                var tableResult = await _dataTableService.GetCurentTopicTable(new TopicTableQuery() { CtlId = ctlId, PageSize = 100000 });
                var metadataTableEntities = tableResult.Data;
                List<DataTableEntity> metadataTableDtoList = new List<DataTableEntity>();
                data.MetadataTableDtoList.ForEach(ditem => metadataTableDtoList.Add(ditem));
                // 该主题下所有的 API URL
                var urlList = new List<string>();
                var delApiTable = new List<string>();
                foreach (var tableEntity in metadataTableEntities)
                {
                    //有的表不在重新生产，剔除重新生产清单
                    var dto = metadataTableDtoList.FirstOrDefault(f => f.Id == tableEntity.Id);
                    if (dto != null)
                    {
                        metadataTableDtoList.Remove(dto);//原先表里面有的，移除不做处理
                        continue;
                    }
                    string apiUrl = $"/{tableEntity.SourceName.Replace("_", "/")}/{tableEntity.TableName}";
                    urlList.Add($"{apiUrl}/sqlQuery".ToLower());
                    urlList.Add($"{apiUrl}/query".ToLower());
                    delApiTable.Add(tableEntity.Id);
                }

                // 删除移除的表
                await CurrentDb.Deleteable<DataCatalogTableMapping>().Where(it => it.CatalogId == ctlId && delApiTable.Contains(it.TableId)).ExecuteCommandAsync();
                if (data.MetadataTableDtoList.Count <= 0)
                {
                    if (urlList.Count() > 0) await _apiService.DeleteByUrlList(urlList);
                    uow.Commit();
                    //await _dataTableService.InitRedisHash();
                    //await _dataCatalogService.InitRedisHash();
                    return true;
                }

                // 添加新的关联关系【前端传入】
                //List<DataTableEntity> metadataTableDtoList = data.MetadataTableDtoList;
                List<APIAutoGenParam> paramsList = new List<APIAutoGenParam>();
                //var list = new List<DataCatalogTableMapping>();
                var all = await _dataCatalogService.GetParentTopic(new[] { ctlId });
                DataCatalogEntity one = all.FirstOrDefault(f => f.Id == ctlId);
                if (one == null)
                {
                    throw new AppFriendlyException($"{ctlId} Topic disabling has not taken effect and cannot be configured.", 5404);
                }


                var topicName = new StringBuilder();
                for (int i = 1; i < all.Count; i++)
                {
                    topicName.Append(all[i].Name).Append("-");
                }

                if (metadataTableDtoList.Any())
                {
                    foreach (var mappingDto in metadataTableDtoList)
                    {
                        var entity = new DataCatalogTableMapping
                        {
                            CatalogId = ctlId,
                            TableId = mappingDto.Id
                        };
                        //list.Add(entity);
                        await this.CurrentDb.Insertable(entity).ExecuteCommandAsync();

                        var param = new APIAutoGenParam
                        {
                            ApiName = $"{topicName}{mappingDto.TableName}",
                            ApiUrl = $"/{mappingDto.SourceName.Replace("_", "/")}/{mappingDto.TableName}",
                            TableId = mappingDto.Id,
                            Remark = $"生成方式：系统自动生成\n" +
                                     $"主题业务含义：{one.Remark}\n" +
                                     $"主题英文：{one.Code}\n" +
                                     $"数据资产主题域：{topicName}{mappingDto.TableName}"
                        };
                        paramsList.Add(param);

                        // 如果 urlList 存在当前需要修改或编辑的 API 路径则从 urlList 中删除。除此之外剩下的是取消关联的 API，则删除
                        urlList.RemoveAll(url => url.Equals($"{param.ApiUrl}/query", StringComparison.CurrentCultureIgnoreCase) || url.Equals($"{param.ApiUrl}/sqlquery", StringComparison.CurrentCultureIgnoreCase));
                    }
                }

                _logger.LogInformation("主题域关联表数量：{0}", metadataTableDtoList.Count);
                _logger.LogInformation("主题域取消关联API数量：{0}", urlList.Count);

                uow.Commit();

                if (urlList.Count <= 0 && paramsList.Count <= 0) return true;
                try
                {
                    var future = Task.Run(async () =>
                    // // 不改变表结构不需要刷新
                    //{
                    //    await _dataTableService.InitRedisHash();
                    //    await _dataCatalogService.InitRedisHash();
                    //}).ContinueWith(async res =>
                    {
                        var a = await _apiService.DeleteByUrlList(urlList);
                        _logger.LogInformation("删除线程:{0}.结果：{1}", Environment.CurrentManagedThreadId, a.Success);
                        return a.Success;
                    }).ContinueWith(async res =>
                    {
                        var b = await _apiService.AutoCreate(paramsList);
                        _logger.LogInformation("自动创建线程:{0}.结果：{1}", Environment.CurrentManagedThreadId, b.Success);
                        return b.Success;
                    });
                }
                catch (Exception e)
                {
                    _logger.LogError("Global Exception!ex={0}, StackTrace={1}", e.Message, e.StackTrace);
                    throw;
                }
            }
            return true;
        }
    }
}
