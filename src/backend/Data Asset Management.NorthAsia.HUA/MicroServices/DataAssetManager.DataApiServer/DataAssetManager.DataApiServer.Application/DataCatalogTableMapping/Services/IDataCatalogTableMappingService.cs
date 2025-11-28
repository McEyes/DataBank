using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataChangeRecord.Dtos;
using DataAssetManager.DataApiServer.Application.DataCatalog.Dtos;

namespace DataAssetManager.DataApiServer.Application
{
    public interface IDataCatalogTableMappingService
    {
        Task<bool> SaveMapping(CatalogMappingDto data);
    }
}
