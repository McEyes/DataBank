using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace ITPortal.Core.SqlParse
{
    // MySQL 
    public class MySqlParser : SqlParser
    {
        public MySqlParser(DbType sugerDbType) : base(sugerDbType)
        {
        }

        public override StringBuilder GetTableColumnsSql(String dbName, String tableName, string schemaName = "public")
        {
            return new StringBuilder(@$"SELECT 
    COLUMN_NAME AS ColName,
    DATA_TYPE AS DataType,
    CHARACTER_MAXIMUM_LENGTH AS DataLength,
    NUMERIC_PRECISION AS DataPrecision,
    NUMERIC_SCALE AS DataScale,
    COLUMN_KEY = 'PRI' AS ColKey,
    IS_NULLABLE = 'YES' AS Nullable,
    ORDINAL_POSITION AS ColPosition,
    COLUMN_DEFAULT AS DataDefault,
    COLUMN_COMMENT AS ColComment
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_SCHEMA = '{dbName}'
    AND TABLE_NAME = '{tableName}';");
        }

        public override StringBuilder GetTablesSql(String dbName, string schemaName = "public")
        {
            return new StringBuilder(@$"SELECT 
    TABLE_NAME as TableName, 
    TABLE_COMMENT as TableComment
FROM 
    INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE!='VIEW' AND 
    TABLE_SCHEMA = '{dbName}';");
        }
    }

}