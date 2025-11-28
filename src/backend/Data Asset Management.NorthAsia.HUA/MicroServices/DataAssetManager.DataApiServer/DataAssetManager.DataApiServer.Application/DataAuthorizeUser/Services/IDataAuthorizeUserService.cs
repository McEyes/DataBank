using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using ITPortal.Core.Services;

namespace DataAssetManager.DataTableServer.Application
{
    public interface IDataAuthorizeUserService : IBaseService<DataAuthorizeUserEntity, DataAuthorizeUserDto, string>
    {
        Task<int> DeleteByUser(string userid);
        Task SaveAuth(AuthUserTableDto input);
        Task<PageResult<DataAuthUserTableDto>> UserAuthList(PageEntity<string> filter);
    }
}
