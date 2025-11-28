using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/Clients/", Name = "数据资产AssetClients服务")]
    [ApiDescriptionSettings(GroupName = "数据资产AssetClients")]
    public class AssetClientsAppService : IDynamicApiController
    {
        private readonly IAssetClientsService _AssetClientsService;
        private readonly IDistributedCacheService _cache;

        public AssetClientsAppService(IAssetClientsService dataApiService, IDistributedCacheService cache)
        {
            _AssetClientsService = dataApiService;
            _cache = cache;
        }

        [HttpGet("SelfAppList")]
        public async Task<List<AssetClientDto>> GetApplictionList(string appName)
        {
            return await _AssetClientsService.GetApplictionList(appName, _AssetClientsService.CurrentUser.UserId);
        }

        [HttpPost("PageList")]
        public async Task<PageResult<AssetClientView>> QueryClientAndSecrets(AssetClientQueryDto entity)
        {
            if (!_AssetClientsService.CurrentUser.IsDataAssetManager)
            {
                entity.Owner = _AssetClientsService.CurrentUser.Id;
            }
            return await _AssetClientsService.QueryClientAndSecrets(entity);
        }

        [HttpPost("PageSelfList")]
        public async Task<PageResult<AssetClientView>> QuerySelfClientAndSecrets(AssetClientQueryDto entity)
        {
            entity.Owner = _AssetClientsService.CurrentUser.Id;
            return await _AssetClientsService.QueryClientAndSecrets(entity);
        }

        [HttpGet("AllAppList")]
        public async Task<List<AssetClientDto>> GetAllApplictionList(string appName)
        {
            return await _AssetClientsService.GetApplictionList(appName, string.Empty);
        }

        [HttpGet("{client_id}/ClientInfo")]
        public async Task<List<AssetClientDto>> GetClientByClientId(string client_id)
        {
            return await _AssetClientsService.GetClientByClientId(client_id);
        }


        [HttpGet("{client_id}/Scopes/{tableId}")]
        public async Task<AssetClientScopesEntity> GetClientScopesByClientId(string client_id, string tableId)
        {
            return await _AssetClientsService.GetClientScopesByClientId(client_id, tableId);
        }


        [HttpPost("CreateSecret")]
        public async Task<ITPortal.Core.Services.IResult> CreateClientSecret(AssetClientEntity createSecret)
        {
            return await _AssetClientsService.CreateClientSecret(createSecret);
        }

        [HttpPost("InitClientScopes")]
        public async Task<List<AssetClientDto>> InitClientScopes(bool clearCache = false)
        {
            return await _AssetClientsService.InitClientScopes(clearCache);
        }

        #region base api


        [HttpPost()]
        public async Task<int> Post(AssetClientEntity entity)
        {
            return await _AssetClientsService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _AssetClientsService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<AssetClientEntity> Get(Guid id)
        {
            return await _AssetClientsService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(dynamic[] ids)
        {
            return await _AssetClientsService.Delete(ids);
        }


        public async Task<AssetClientEntity> Single(AssetClientQueryDto entity)
        {
            return await _AssetClientsService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(AssetClientEntity entity)
        {
            return await _AssetClientsService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(AssetClientEntity entity)
        {
            return await _AssetClientsService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<AssetClientEntity>> Page([FromQuery] AssetClientQueryDto filter)
        {
            return await _AssetClientsService.Page(filter);
        }

        [HttpPost("page")]
        public async Task<PageResult<AssetClientEntity>> Page2(AssetClientQueryDto filter)
        {
            return await _AssetClientsService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<AssetClientEntity>> Query([FromQuery] AssetClientQueryDto entity)
        {
            return await _AssetClientsService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<AssetClientEntity>> Query2(AssetClientQueryDto entity)
        {
            return await _AssetClientsService.Query(entity);
        }

        #endregion base api
    }
}
