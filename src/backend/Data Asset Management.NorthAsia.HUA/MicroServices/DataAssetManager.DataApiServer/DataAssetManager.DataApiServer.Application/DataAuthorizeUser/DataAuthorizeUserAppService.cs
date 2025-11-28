using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
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
    [Route("api/authorizes/", Name = "数据资产 DataAuthorizeUser 服务")]
    [ApiDescriptionSettings(GroupName = "数据资产 DataAuthorizeUser")]
    public class DataAuthorizeUserAppService : IDynamicApiController
    {
        private readonly IDataAuthorizeUserService _dataAuthorizeUserService;
        private readonly IDataApiService _dataApiService;
        private readonly IDistributedCacheService _cache;

        public DataAuthorizeUserAppService(IDataAuthorizeUserService dataApiService, IDataApiService dataApiService1, IDistributedCacheService cache)
        {
            _dataAuthorizeUserService = dataApiService;
            _cache = cache;
            _dataApiService = dataApiService1;
        }


        [HttpGet("userAuthList")]
        [HttpPost("userAuthList")]
        public async Task<PageResult<DataAuthUserTableDto>> UserAuthList([FromQuery]PageEntity<string> filter)
        {
            return await _dataAuthorizeUserService.UserAuthList(filter);
        }

        [HttpPost("saveAuth")]
        public async Task SaveAuth(AuthUserTableDto input)
        {
             await _dataAuthorizeUserService.SaveAuth(input);
        }


        [HttpDelete("user/{userid}")]
        public async Task<int> SaveAuth(string userid)
        {
            return await _dataAuthorizeUserService.DeleteByUser(userid);
        }


        #region base api


        [HttpPost()]
        public async Task<int> Create(DataAuthorizeUserEntity entity)
        {
            return await _dataAuthorizeUserService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<int> Delete(string id)
        {
            return await _dataAuthorizeUserService.DeleteByUser(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DataAuthorizeUserEntity> Get(string id)
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


        public async Task<DataAuthorizeUserEntity> Single(DataAuthorizeUserDto entity)
        {
            return await _dataAuthorizeUserService.Single(entity);
        }

        [HttpPut()]
        public async Task<int> Put(DataAuthorizeUserEntity entity)
        {
            return await _dataAuthorizeUserService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(DataAuthorizeUserEntity entity)
        {
            return await _dataAuthorizeUserService.ModifyHasChange(entity);
        }


        [HttpPost("page")]
        public async Task<PageResult<DataAuthorizeUserEntity>> Page(DataAuthorizeUserDto filter)
        {
            return await _dataAuthorizeUserService.Page(filter);
        }

        [HttpGet("page")]
        public async Task<PageResult<DataAuthorizeUserEntity>> Page2([FromQuery] DataAuthorizeUserDto filter)
        {
            return await _dataAuthorizeUserService.Page(filter);
        }

        [HttpPost("list")]
        public async Task<List<DataAuthorizeUserEntity>> Query(DataAuthorizeUserDto entity)
        {
            return await _dataAuthorizeUserService.Query(entity);
        }

        [HttpGet("list")]
        public async Task<List<DataAuthorizeUserEntity>> Query2([FromQuery] DataAuthorizeUserDto entity)
        {
            return await _dataAuthorizeUserService.Query(entity);
        }

        #endregion base api
    }
}
