using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataChangeRecord.Dtos;
using DataAssetManager.DataApiServer.Core;

namespace DataAssetManager.DataApiServer.Application
{
    public interface IDataChangeRecordService : IBaseService<DataChangeRecordEntity, DataChangeRecordDto, string>
    {
        Task<List<DataChangeRecordEntity>> GetByTableId(string objectType);
        Task<int> SaveBatch(List<DataChangeRecordEntity> columns);
    }
}
