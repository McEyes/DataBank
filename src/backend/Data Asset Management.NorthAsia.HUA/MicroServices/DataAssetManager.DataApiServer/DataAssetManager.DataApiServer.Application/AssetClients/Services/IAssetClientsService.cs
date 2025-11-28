using DataAssetManager.DataApiServer.Application.AssetClients.Dtos;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;

using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application
{
    public interface IAssetClientsService : IBaseService<AssetClientEntity, AssetClientQueryDto, Guid>
    {
        Task<List<AssetClientDto>> AllClients();
        Task<List<AssetClientDto>> AllClientScopes();
        Task<List<AssetClientDto>> AllClientScopesFromCache(bool clearCache = false);
        Task<List<AssetClientDto>> AllClientsFromCache(bool clearCache = false);
        Task<List<AssetClientScopesEntity>> AllScopes();
        Task<Dictionary<Guid, List<AssetClientScopesEntity>>> AllScopesDictFromCache(bool clearCache = false);
        Task<List<AssetClientScopesEntity>> AllScopesFromCache(bool clearCache = false);
        Task<List<AssetClientSecretsEntity>> AllSecrets();
        Task<Dictionary<Guid, List<AssetClientSecretsEntity>>> AllSecretsDictFromCache(bool clearCache = false);
        Task<List<AssetClientSecretsEntity>> AllSecretsFromCache(bool clearCache = false);
        Task<Result<List<AssetClientScopesEntity>>> CheckAuth(DataAuthCheckDto applyInfo);
        Task<ITPortal.Core.Services.IResult<string>> CreateClientInfo(ApplyAuthCallbackDto createSecret);
        Task<ITPortal.Core.Services.IResult> CreateClientSecret(AssetClientEntity createSecret);
        Task<List<AssetClientDto>> GetApplictionList(string appName, string userid);
        Task<List<AssetClientDto>> GetClientByClientId(string client_id);
        Task<List<AssetClientDto>> GetClientByClientId(string client_id, int clientType = 1);
        Task<AssetClientScopesEntity> GetClientScopesByClientId(string client_id, string tableId);
        Task<AssetClientSecretsEntity> GetClientSecretsByClientId(string client_id, string secret);
        Task<Result<List<UserInfo>>> GetUsersAsync();
        Task<List<AssetClientDto>> InitClientScopes(bool clearCache = false);
        Task<PageResult<AssetClientView>> QueryClientAndSecrets(AssetClientQueryDto filter);
        Task<ITPortal.Core.Services.IResult> UpdataClientScopes(AuthUserTableDto input);
    }
}
