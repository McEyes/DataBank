using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;

namespace DataAssetManager.DataApiServer.Application
{
    public interface IDataColumnService : IBaseService<DataColumnEntity, DataColumnDto, string>
    {
        Task<int> DeleteByTableId(string tableId);
        Task<int> DeleteByTableId(dynamic[] tableIds);

        //Task<List<DataColumnEntity>> All();
        //Task<List<DataColumnEntity>> AllFromCache();
        Task<List<DataColumnEntity>> GetByTableId(string tableId);
        Task<int> SaveBatch(List<DataColumnEntity> columns);
    }
}
