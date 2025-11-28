using ITPortal.Core.Services;

using MetadataManagement.Core.Entitys;

namespace MetadataManagement.Application
{
    public interface IDataColumnService : IBaseService<DataColumnEntity, DataColumnQuery, long>
    {
    }
}
