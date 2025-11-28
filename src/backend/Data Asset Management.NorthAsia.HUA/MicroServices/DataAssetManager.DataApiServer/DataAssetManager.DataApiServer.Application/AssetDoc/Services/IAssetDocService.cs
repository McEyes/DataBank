using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;

using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataTableServer.Application
{
    public interface IAssetDocService : IBaseService<AssetDocEntity, AssetDocDto, Guid>
    {
    }
}
