using ITPortal.Core.Services;

using MetadataManagement.Core.Entitys;

namespace MetadataManagement.Application
{
    public interface IDataSourceService : IBaseService<DataSourceEntity, DataSourceQuery, long>
    {
    }
}
