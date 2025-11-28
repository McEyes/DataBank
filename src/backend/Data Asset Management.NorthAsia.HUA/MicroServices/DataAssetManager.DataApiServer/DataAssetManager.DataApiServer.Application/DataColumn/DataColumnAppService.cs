using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/columns/", Name = "数据资产DataColumn服务")]
    [ApiDescriptionSettings(GroupName = "数据资产DataColumn")]
    public class DataColumnAppService : IDynamicApiController
    {
        private readonly IDataColumnService _dataColumnService;
        private readonly IDistributedCacheService _cache;

        public DataColumnAppService(IDataColumnService dataApiService, IDistributedCacheService cache)
        {
            _dataColumnService = dataApiService;
            _cache = cache;
        }


        #region base api


        [HttpPost()]
        public async Task<int> Post(DataColumnEntity entity)
        {
            if (entity.Id.IsNullOrWhiteSpace())
                return await _dataColumnService.Modify(entity);
            else return await _dataColumnService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _dataColumnService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DataColumnEntity> Get(string id)
        {
            return await _dataColumnService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("Batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _dataColumnService.Delete(ids);
        }


        public async Task<DataColumnEntity> Single(DataColumnDto entity)
        {
            return await _dataColumnService.Single(entity);
        }

        [HttpPut()]
        [HttpPut("{Id}")]
        public async Task<int> Put(DataColumnEntity entity)
        {
            return await _dataColumnService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(DataColumnEntity entity)
        {
            return await _dataColumnService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<DataColumnEntity>> Page([FromQuery] DataColumnDto filter)
        {
            return await _dataColumnService.Page(filter);
        }

        [HttpPost("page")]
        public async Task<PageResult<DataColumnEntity>> Page2(DataColumnDto filter)
        {
            return await _dataColumnService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<DataColumnEntity>> Query([FromQuery] DataColumnDto entity)
        {
            return await _dataColumnService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<DataColumnEntity>> Query2(DataColumnDto entity)
        {
            return await _dataColumnService.Query(entity);
        }

        #endregion base api
    }
}
