using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataAuthorizeUser.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;

using ITPortal.Core.Services;

namespace DataAssetManager.DataTableServer.Application
{
    public interface IDataTableService : IBaseService<DataTableEntity, DataTableInfo, string>
    {
        Task<List<DataTableInfo>> All();
        Task<List<DataTableInfo>> AllFromCache(bool clearCache = false);

        Task<List<DataTableAuthorizeUser>> TableUserAll();
        Task<List<List<string>>> InitTableUserFromCache(bool clearCache = false);
        List<string> GetTableAuthorizeUsers(string tableId);
        bool CheckUserHasTable(string tableId, string userid);
        Task<PageResult<DataTableInfo>> GetTablesByTag(TopicTableQuery filter);
        Task<PageResult<DataTableInfo>> GetTopicTable(TopicTableQuery filter);
        Task<List<string>> GetTableTags();
        Task<List<DataTableInfo>> InitRedisHash(bool clearCache = false);
        Task<PageResult<DataTableInfo>> GetUserTable(PageEntity<string> filter);
        Task<List<DataTableInfo>> GetUserAuthTablesByUserId(string userid);
        Task<DataTableEntity> Save(DataTableInput entity);
        Task<DataTableInput> GetInfo(string id);
        Task<DataTableEntity> Update(DataTableInput entity);
        Task<List<DataTableInfo>> GetTableByTopic(TopicTableQuery filter);
        Task<DataTableInfo> GetTableInfoByName(string tableName);
        Task<List<DataColumnEntity>> GetTableCloumnsByName(string tableName);
        Task<List<DataColumnEntity>> GetTableCloumnsByTableId(string tableId);
        Task<List<string>> GetTableDepts();
        Task<List<TableApiVisitStatics>> GetTableStatistics();
        Task<List<TableStandardizedRateEntity>> GetStandardizationStatistics(string dept);
        Task<PageResult<DataTableInfo>> GetCurentTopicTable(TopicTableQuery filter);
        Task<long> GetCategoryMapTableCount();
        Task<long> GetCategoryMapColumnCount();
        Task<long> GetCategoryMapApiCount();
        Task<List<string>> GetCategoryMapApiList();
        Task<List<string>> GetCategoryMapTableIdList();
        Task<Dictionary<string, HashSet<string>>> GetAllTableAlias(bool clearCache = false);
        Task<HashSet<string>> GetTableAlias(string tableName, bool clearCache = false);
        Task<bool> CheckIsTableAlias(string tableName, string tableAlias, bool clearCache = false);
        Task<Dictionary<string, DataTableRename>> GetAllTableRename(bool clearCache = false);
        Task<DataTableRename> GetTableRename(string sourceName, string tableName, bool clearCache = false);
        Task<PageResult<DataTableInfo>> GetOwnerTables(DataTableDto filter);
        //Task<Dictionary<string, HashSet<string>>> GetTopTopicApiList(string[] ctlIds=null, string searchkey="");
    }
}
