using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;

using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataTableServer.Application
{
    public interface IDataUserService : IBaseService<MetaDataUserEntity, MetaDataUserDto, string>
    {
        Task<List<ApproveDto>> GetTableApprovers(string tableId);
        Task<List<MetaDataUserEntity>> GetDataUserByTableId(string tableId);
        Task<int> DeleteByTableId(string tableId);
        Task<int> DeleteByTableId(dynamic[] tableId);
    }
}
