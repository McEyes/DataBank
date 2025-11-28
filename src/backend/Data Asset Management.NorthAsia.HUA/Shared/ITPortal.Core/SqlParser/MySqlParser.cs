using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ITPortal.Core.SqlParse
{
    // MySQL ������ʵ��
    public class MySqlParser : SqlParser
    {
        public MySqlParser(SqlSugar.DbType sugerDbType) : base(sugerDbType)
        {
        }

        public override StringBuilder GetTableColumnsSql(String dbName, String tableName, string schemaName = "public")
        {
            if ("vldb".Equals(dbName, StringComparison.CurrentCultureIgnoreCase))
            {
                return new StringBuilder(@$"SELECT
    colname AS ColName,
    datatype AS DataType,
    datalength AS DataLength,
    dataprecision AS DataPrecision,
    datascale AS DataScale,
    colkey AS ColKey,
    nullable AS Nullable,
    colposition AS ColPosition,
    datadefault AS DataDefault,
    '' AS ColComment
FROM vldb_columns
WHERE catalog_tablename = '{tableName}' order by colposition;");
            }
            else
            {
                return new StringBuilder(@$"SELECT 
    COLUMN_NAME AS ColName,
    DATA_TYPE AS DataType,
    CHARACTER_MAXIMUM_LENGTH AS DataLength,
    NUMERIC_PRECISION AS DataPrecision,
    NUMERIC_SCALE AS DataScale,
    COLUMN_KEY = 'PRI' AS ColKey,
    case when is_nullable = 'YES' then 1 else 0 end  AS Nullable,
    ORDINAL_POSITION AS ColPosition,
    COLUMN_DEFAULT AS DataDefault,
    COLUMN_COMMENT AS ColComment
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_SCHEMA = '{dbName}'
    AND TABLE_NAME = '{tableName}';");
            }
        }

        public override StringBuilder GetTablesSql(String dbName, string schemaName = "public")
        {
            if ("vldb".Equals(dbName, StringComparison.CurrentCultureIgnoreCase))
            {
                return new StringBuilder(@$"SELECT CATALOG_TABLENAME AS TableName,
TABLE_NAME AS TableComment
FROM vldb.vldb_tables {(schemaName != "public" ? $" WHERE TABLE_SCHEMA='{schemaName}'" : "")};");
            }
            else
            {
                return new StringBuilder(@$"SELECT 
    TABLE_NAME as TableName, 
    TABLE_COMMENT as TableComment
FROM 
    INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = '{dbName}';");
            }
        }
    }
}