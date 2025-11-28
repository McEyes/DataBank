using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using Microsoft.Extensions.Logging;

using StackExchange.Profiling.Internal;

using System.Collections.Generic;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/assetCatalog/", Name = "数据资产TableCatalog服务")]
    [ApiDescriptionSettings(GroupName = "数据资产TableCatalog")]
    public class DataCatalogAppService : IDynamicApiController
    {
        private readonly IDataCatalogService _dataCatalogService;
        private readonly IDataTableService _dataTableService;
        private readonly IDataCatalogTableMappingService _catalogTableMappingService;
        private readonly IDistributedCacheService _cache;
        private readonly ILogger<DataTableService> _logger;

        public DataCatalogAppService(IDataCatalogService dataApiService, IDataTableService dataTableService, IDataCatalogTableMappingService catalogTableMappingService, IDistributedCacheService cache, ILogger<DataTableService> logger)
        {
            _dataCatalogService = dataApiService;
            _dataTableService = dataTableService;
            _catalogTableMappingService = catalogTableMappingService;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("topic")]
        [HttpGet("getTopic")]
        public async Task<List<DataCatalogEntity>> GetTopic(string[]? ctlIds, string? searchkey)
        {
            return await _dataCatalogService.GetTopic(ctlIds, searchkey);
        }

        [HttpGet("topic/Childrens")]
        [HttpGet("GetTopic/Childrens")]
        public async Task<List<DataCatalogEntity>> GetChildrensTopic(string[]? ctlIds, string? searchkey)
        {
            return await _dataCatalogService.GetChildrensTopic(ctlIds, searchkey);
        }

        [HttpGet("tree/topic")]
        [HttpGet("topic/tree")]
        public async Task<List<TreeEntity>> GetTreeTopic(string[]? ctlIds, string? searchkey)
        {
            Func<Task<List<TreeEntity>>> getData = async () =>
            {
                List<TreeEntity> list = new List<TreeEntity>();
                if ((ctlIds == null || ctlIds.Length == 0) && searchkey.IsNullOrWhiteSpace())
                    list = await _dataCatalogService.GetTreeTopicFromCache();
                list = await _dataCatalogService.GetTreeTopic(ctlIds, searchkey);

                var tables = await _dataTableService.AllFromCache();
                //var catalogTables = tables.GroupBy(f => f.CtlId).Select(d => new { d.Key, count = d.Count() });

                list.ForEach(f =>
                {
                    GetChildCount(f, f.Children, tables);
                });
                return list;
            };

            var cacheKey = $"{DataAssetManagerConst.RedisKey}DataCatalog:tree_topic";
            if (!searchkey.IsNullOrWhiteSpace()) cacheKey = cacheKey + searchkey;
            if (ctlIds != null && ctlIds.Count() > 0) cacheKey = cacheKey + string.Join("_", ctlIds);

            if (cacheKey.Length > 64) return await getData();
            else return await _cache.GetObjectAsync(cacheKey, getData, TimeSpan.FromSeconds(60));
        }


        private int GetChildCount(TreeEntity pentity, List<TreeEntity> childrens, List<DataTableInfo> tables)
        {
            foreach (TreeEntity child in childrens)
            {
                var list = tables.Where(d => child.Key == d.CtlId);
                child.Count = list.Count() + GetChildCount(child, child.Children, tables);
                child.Children.AddRange(list.Select(f => new TreeEntity()
                {
                    Key = f.Id,
                    PId = f.CtlId,
                    Type = "table",
                    Code = f.TableName,
                    Label = f.TableComment,
                    Value = f.TableName,
                }));
            }
            var childrenCount = childrens.Sum(f => f.Count);
            pentity.Count = tables.Where(d => pentity.Key == d.CtlId).Count() + childrenCount;
            return childrenCount;
        }


        [HttpGet("tree/table")]
        public async Task<List<TreeEntity>> GetTreeTable(string[]? ctlIds, string? searchkey)
        {
            Func<Task<List<TreeEntity>>> getData = async () =>
            {
                List<TreeEntity> list = new List<TreeEntity>();
                if ((ctlIds == null || ctlIds.Length == 0) && searchkey.IsNullOrWhiteSpace())
                    list = await _dataCatalogService.GetTreeTopicFromCache();
                list = await _dataCatalogService.GetTreeTopic(ctlIds, searchkey);
                var tables = await _dataTableService.AllFromCache();
                list.ForEach(f =>
                {
                    GetChild(f, f.Children, tables);
                });
                return list;
            };

            var cacheKey = $"{DataAssetManagerConst.RedisKey}DataCatalog:tree_table";
            if (!searchkey.IsNullOrWhiteSpace()) cacheKey = cacheKey + searchkey;
            if (ctlIds != null && ctlIds.Count() > 0) cacheKey = cacheKey + string.Join("_", ctlIds);

            if (cacheKey.Length > 64) return await getData();
            else return await _cache.GetObjectAsync(cacheKey, getData, TimeSpan.FromSeconds(60));
        }

        private void GetChild(TreeEntity pentity, List<TreeEntity> childrens, List<DataTableInfo> tables)
        {
            foreach (TreeEntity child in childrens)
            {
                var tableList = tables.Where(d => child.Key == d.CtlId);
                if (tableList.Count() > 0) child.Children = tableList.Select(f => new TreeEntity()
                {
                    Key = f.Id,
                    Code = f.TableName,
                    Value = f.TableName,
                    Label = f.TableName,
                    Type = "table",
                    PId = child.Key,
                }).ToList();
                GetChildCount(child, child.Children, tables);
            }
        }


        [HttpPost("SaveMapping")]
        public async Task SaveMapping(CatalogMappingDto data)
        {
            await _catalogTableMappingService.SaveMapping(data);
        }

        #region base api


        [HttpPost()]
        public async Task<int> Post(DataCatalogDto entity)
        {
            //if (entity.Id.IsNullOrWhiteSpace()) entity.Id = Guid.NewGuid().ToString();
            entity.UpdateBy = _dataCatalogService.CurrentUser?.UserName ?? "test";
            entity.UpdateTime = DateTimeOffset.Now;
            return await _dataCatalogService.Create(entity.Adapt<DataCatalogEntity>());
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _dataCatalogService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DataCatalogEntity> Get(string id)
        {
            return await _dataCatalogService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("Batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _dataCatalogService.Delete(ids);
        }


        public async Task<DataCatalogEntity> Single(DataCatalogDto entity)
        {
            return await _dataCatalogService.Single(entity);
        }

        [HttpPut()]
        [HttpPut("{id}")]
        public async Task<int> Put(DataCatalogEntity entity)
        {
            return await _dataCatalogService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        [HttpPut("ModifyHasChange/{id}")]
        public async Task<bool> ModifyHasChange(DataCatalogEntity entity)
        {
            return await _dataCatalogService.ModifyHasChange(entity);
        }

        [HttpPost("page")]
        public async Task<PageResult<DataCatalogInfo>> Page(DataCatalogDto filter)
        {
            var result = await _dataCatalogService.QueryPage(filter);

            var tables = await _dataTableService.AllFromCache();// _cache.GetObjectAsync<List<DataTableInfo>>(DataAssetManagerConst.DataTable_ListKey);
            result.Data.ForEach(item =>
            {
                item.Tables = tables.Where(f => f.CtlId == item.Id).ToList();
            });
            return result;
        }

        [HttpGet("page")]
        public async Task<PageResult<DataCatalogInfo>> Page2([FromQuery] DataCatalogDto filter)
        {
            var result = await _dataCatalogService.QueryPage(filter);

            var tables = await _dataTableService.AllFromCache();//.GetObjectAsync<List<DataTableInfo>>(DataAssetManagerConst.DataTable_ListKey);
            result.Data.ForEach(item =>
            {
                item.Tables = tables.Where(f => f.CtlId == item.Id).ToList();
            });
            return result;
        }

        [HttpGet("list")]
        [HttpPost("list")]
        public async Task<List<DataCatalogEntity>> Query(bool clearCache = false)
        {
            return await _dataCatalogService.AllFromCache(clearCache);
        }

        #endregion base api
    }
}
