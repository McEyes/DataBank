using System.Text;

namespace ITPortal.Core.SqlParse
{
    public interface ISqlParser
    {
        string[] PagePattern { get; }
        //string Sql { get; }
        //HashSet<string> TableNames { get; }
        //(int pageNum,int PageSize) PageInfo { get; }
        //ISqlParser Parse(string sql);

        //ISqlParser RemovePageInfo();
        //string ParamSymbols { get; }
        List<string> ExtractTableNames(string sql);
        (bool hasPagination, string paginationInfo) ExtractPaginationInfo(string sql);
        List<string> ExtractFields(string sql);
        List<string> ExtractConditions(string sql);
        string ReplaceConditions(string sql, Dictionary<string, object> parameters);
        string BuildConditions<T>(List<T> fieldParam) where T : class, IReqParam, new();//, Func<T, string> expression) where T : class, IReqParam, new();
        StringBuilder BuildCondition<T>(T fieldParam, StringBuilder sqlBuild) where T : class, IReqParam, new();

        string BuildConditions<T>(List<T> fieldParam, Dictionary<string, object> parameters) where T : class, IReqParam, new();//, Func<T, string> expression) where T : class, IReqParam, new();
        StringBuilder BuildCondition<T>(T fieldParam, StringBuilder sqlBuild, object data_start, object data_end = null) where T : class, IReqParam, new();
        string RemovePagination(string sql);
        string GetSqlType(string sql);
        //ISugarQueryable<object> BuildSelectFromTable(ISqlSugarClient db, string tableName, string[] columns=null);
        //StringBuilder BuildSelectFromTableToSql(ISqlSugarClient db, string tableName, string[] columns = null);
        bool HasPagination(string sql, out string paginatedSql);
        StringBuilder GetTableColumnsSql(string dbName, string tableName, string schemaName = "public");
        StringBuilder GetTablesSql(string dbName, string schemaName = "public");
        string ReplaceFields(string sql, string selectFrom);
        string ReplaceFields(string sql, List<string> columns);
    }
}
