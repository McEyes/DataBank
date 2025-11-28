using System.Data;
using System.Text;

namespace ITPortal.Core.SqlParse
{
    // SQLite ������ʵ��
    public class SqliteParser : SqlParser
    {
        public SqliteParser(DbType sugerDbType) : base(sugerDbType)
        {
        }



        public override StringBuilder GetTableColumnsSql(String dbName, String tableName, string schemaName = "public")
        {
            return new StringBuilder(@$"PRAGMA table_info(tableName);");
        }

        public override StringBuilder GetTablesSql(String dbName, string schemaName = "public")
        {
            return new StringBuilder(@$"SELECT 
    name AS TableName,'' AS TableComment
FROM 
    sqlite_master 
WHERE 
    type IN ('table', 'view');");
        }
    }
}