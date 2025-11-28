using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using ITPortal.Core.Services;

namespace DataAssetManager.DataTableServer.Application
{
    public interface IDataAccessService
    {
        Task<dynamic> GetDataPreview(string tableId, int pageSize = 20, string sort = "");
        Task<dynamic> SqlRun(SqlDto dto);
    }
}
