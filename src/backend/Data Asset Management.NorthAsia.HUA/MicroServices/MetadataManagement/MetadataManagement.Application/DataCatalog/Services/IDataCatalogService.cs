using ITPortal.Core.Services;

using MetadataManagement.Core.Entitys;

namespace MetadataManagement.Application
{
    public interface IDataCatalogService : IBaseService<DataCatalogEntity, DataCatalogQuery, long>
    {
    }
}
