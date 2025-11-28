using DataAssetManager.DataApiServer.Application.DataChangeRecord.Dtos;
using DataAssetManager.DataApiServer.Core;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/ChangeRecords/", Name = "数据资产DataChangeRecord服务")]
    [ApiDescriptionSettings(GroupName = "数据资产DataChangeRecord")]
    public class DataChangeRecordAppService : IDynamicApiController
    {
        private readonly IDataChangeRecordService _dataChangeRecordService;
        private readonly IDistributedCacheService _cache;

        public DataChangeRecordAppService(IDataChangeRecordService dataApiService, IDistributedCacheService cache)
        {
            _dataChangeRecordService = dataApiService;
            _cache = cache;
        }


        #region base api


        [HttpPost()]
        public async Task<int> Post(DataChangeRecordEntity entity)
        {
            return await _dataChangeRecordService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _dataChangeRecordService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DataChangeRecordEntity> Get(string id)
        {
            return await _dataChangeRecordService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("Batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _dataChangeRecordService.Delete(ids);
        }


        public async Task<DataChangeRecordEntity> Single(DataChangeRecordDto entity)
        {
            return await _dataChangeRecordService.Single(entity);
        }

        [HttpPut()]
        public async Task<int> Put(DataChangeRecordEntity entity)
        {
            return await _dataChangeRecordService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(DataChangeRecordEntity entity)
        {
            return await _dataChangeRecordService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<DataChangeRecordEntity>> Page([FromQuery] DataChangeRecordDto filter)
        {
            return await _dataChangeRecordService.Page(filter);
        }

        [HttpPost("page")]
        public async Task<PageResult<DataChangeRecordEntity>> Page2(DataChangeRecordDto filter)
        {
            return await _dataChangeRecordService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<DataChangeRecordEntity>> Query([FromQuery] DataChangeRecordDto entity)
        {
            return await _dataChangeRecordService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<DataChangeRecordEntity>> Query2(DataChangeRecordDto entity)
        {
            return await _dataChangeRecordService.Query(entity);
        }

        #endregion base api
    }
}
