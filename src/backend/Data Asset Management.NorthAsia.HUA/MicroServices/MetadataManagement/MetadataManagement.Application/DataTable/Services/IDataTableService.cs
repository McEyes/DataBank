using ITPortal.Core.Services;

using MetadataManagement.Core.Entitys;

namespace MetadataManagement.Application
{
    public interface IDataTableService : IBaseService<DataTableEntity, DataTableQuery, long>
    {
    }
}
