using Microsoft.Extensions.FileSystemGlobbing.Internal;

using StackExchange.Profiling.Internal;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ITPortal.Core.SqlParse
{
    // SQLite ������ʵ��
    public class SqliteParser : SqlParser
    {
        public SqliteParser(SqlSugar.DbType sugerDbType) : base(sugerDbType)
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