using System.Threading.Tasks;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataTableServer.Application;

using Furion.JsonSerialization;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Excel;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using MapsterMapper;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/apiLogs/", Name = "数据资产Apilogs服务")]
    [ApiDescriptionSettings(GroupName = "数据资产Apilogs")]
    public class DataApiLogAppService : IDynamicApiController
    {
        private readonly IDataApiLogService _dataApiLogService;
        private readonly IDataApiService _dataApiService;
        private readonly IMapper _mapper;
        private readonly IDistributedCacheService _cache;

        public DataApiLogAppService(IDataApiLogService dataApiService, IDataApiService dataApiService1, IMapper mapper, IDistributedCacheService cache)
        {
            _dataApiLogService = dataApiService;
            _mapper = mapper;
            _cache = cache;
            _dataApiService = dataApiService1;
        }

        //        [HttpGet("getUsageStats")]
        //        public async Task<dynamic> GetUsageStats()
        //        {
        //#if DEBUG
        //            await _cache.RemoveAsync(DataAssetManagerConst.VISUALIZATION_HOT_TABLE_KEY);
        //            await _cache.RemoveAsync(DataAssetManagerConst.VISUALIZATION_API_VISITED_KEY);
        //            await _cache.RemoveAsync(DataAssetManagerConst.VISUALIZATION_CATALOG_VISITED_KEY);
        //            _cache.SetInt(DataAssetManagerConst.VISUALIZATION_TABLE_TOTAL_VISITS_KEY, 10);
        //            _cache.SetObject(DataAssetManagerConst.VISUALIZATION_HOT_TABLE_KEY, new HotTableVisitedDto[] { new HotTableVisitedDto { SourceId = "1765255294130081794", TableName = "Bay主数据", Visited = "2" } });
        //            _cache.SetObject(DataAssetManagerConst.VISUALIZATION_API_VISITED_KEY, new ApiVisitedDto[] { new ApiVisitedDto { ApiId = "1765255294130081794", ApiName = "Bay主数据", Visited = "2" } });
        //            _cache.SetObject(DataAssetManagerConst.VISUALIZATION_CATALOG_VISITED_KEY, new CatalogVisitedDto[] { new CatalogVisitedDto { CtlId = "1863782849254191106", Code = "MES Rework Data", Name = "MES Rework 数据", Visited = "5" } });
        //#endif 
        //            return new
        //            {
        //                TableTotalVisits = await _cache.GetIntAsync(DataAssetManagerConst.VISUALIZATION_TABLE_TOTAL_VISITS_KEY),
        //                HotReadTable = await _cache.GetObjectAsync<List<HotTableVisitedDto>>(DataAssetManagerConst.VISUALIZATION_HOT_TABLE_KEY),
        //                ApiVisited = await _cache.GetObjectAsync< List<ApiVisitedDto>>(DataAssetManagerConst.VISUALIZATION_API_VISITED_KEY),
        //                CatalogVisited = await _cache.GetObjectAsync< List<CatalogVisitedDto>>(DataAssetManagerConst.VISUALIZATION_CATALOG_VISITED_KEY)
        //            };
        //        }


        [HttpGet("registerMapping/{id}")]
        public async Task<string> RegisterMapping(string id)
        {
            _dataApiService.Register(id);
            return await Task.FromResult("Mapping success");
        }

        [HttpPost("ExportExcel"), NonUnify]
        public async Task<IActionResult> ExportExcel(DataApiLogQuery filter)
        {
            var data= await _dataApiLogService.LogViewQuery(filter);
            return ExcelExporter.ExportExcel(data.Data, "ApiLog");
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(DataApiLogEntity entity)
        {
            return await _dataApiLogService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _dataApiLogService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DataApiLogEntity> Get(string id)
        {
            return await _dataApiLogService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _dataApiLogService.Delete(ids);
        }


        public async Task<DataApiLogEntity> Single(DataApiLogDto entity)
        {
            return await _dataApiLogService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(string id, DataApiLogEntity entity)
        {
            if (!id.IsNullOrWhiteSpace()) entity.Id = id;
            return await _dataApiLogService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(DataApiLogEntity entity)
        {
            return await _dataApiLogService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<DataApiLogView>> Page([FromQuery] DataApiLogQuery filter)
        {
            return await _dataApiLogService.LogViewQuery(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<DataApiLogView>> Page2(DataApiLogQuery filter)
        {
            var list = await _dataApiLogService.LogViewQuery(filter);
            return list;
        }

        #endregion base api
    }
}
