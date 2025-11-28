using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using MapsterMapper;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/dataUser/", Name = "数据资产 DataUser 服务")]
    [ApiDescriptionSettings(GroupName = "数据资产 DataUser")]
    public class DataUserAppService : IDynamicApiController
    {
        private readonly IDataUserService _dataAuthorizeUserService;
        private readonly IDistributedCacheService _cache;

        public DataUserAppService(IDataUserService dataApiService,IDistributedCacheService cache)
        {
            _dataAuthorizeUserService = dataApiService;
            _cache = cache;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Post(MetaDataUserEntity entity)
        {
            return await _dataAuthorizeUserService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _dataAuthorizeUserService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<MetaDataUserEntity> Get(string id)
        {
            return await _dataAuthorizeUserService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("Batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _dataAuthorizeUserService.Delete(ids);
        }


        public async Task<MetaDataUserEntity> Single(MetaDataUserDto entity)
        {
            return await _dataAuthorizeUserService.Single(entity);
        }

        [HttpPut()]
        public async Task<int> Put(MetaDataUserEntity entity)
        {
            return await _dataAuthorizeUserService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(MetaDataUserEntity entity)
        {
            return await _dataAuthorizeUserService.ModifyHasChange(entity);
        }

        [HttpPost("page")]
        public async Task<PageResult<MetaDataUserEntity>> Page(MetaDataUserDto filter)
        {
            return await _dataAuthorizeUserService.Page(filter);
        }

        [HttpGet("page")]
        public async Task<PageResult<MetaDataUserEntity>> Page3([FromQuery] MetaDataUserDto filter)
        {
            return await _dataAuthorizeUserService.Page(filter);
        }

        [HttpPost("list")]
        public async Task<List<MetaDataUserEntity>> Query(MetaDataUserDto entity)
        {
            return await _dataAuthorizeUserService.Query(entity);
        }

        [HttpGet("list")]
        public async Task<List<MetaDataUserEntity>> Query3([FromQuery] MetaDataUserDto entity)
        {
            return await _dataAuthorizeUserService.Query(entity);
        }

        #endregion base api
    }
}
