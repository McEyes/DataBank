using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using ITPortal.Core.SqlParser.Models;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;

namespace DataAssetManager.DataApiServer.Application
{
    public interface IDataSourceService : IBaseService<DataSourceEntity, DataSourceDto, string>
    {
        Task<List<DataSourceEntity>> All();
        Task<List<DataSourceEntity>> AllFromCache(bool clearCache = false);
        Task<ITPortal.Core.Services.IResult> CheckConnection(DataSourceDto dataSource);
        Task<int> Count();
        Task<List<DataColumnEntity>> GetDataTableColumns(string sourceId, string tableName);
        Task<List<DataColumnEntity>> GetDbTableColumns(string sourceId, string tableName);
        Task<List<DbTable>> GetDbTablesMergeLocal(string sourceId);
        Task<List<DataSourceEntity>> InitRedisHash();
    }
}
