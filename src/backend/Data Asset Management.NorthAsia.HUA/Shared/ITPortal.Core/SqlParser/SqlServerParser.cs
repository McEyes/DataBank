using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ITPortal.Core.SqlParse
{
    // SQL Server ������ʵ��
    public class SqlServerParser : SqlParser
    {
        public SqlServerParser(SqlSugar.DbType sugerDbType) : base(sugerDbType)
        {
        }

        override public string[] PagePattern =>new string[] { @"(?i)\sOFFSET\s+(\d+)\s+ROWS\s+FETCH\s+NEXT\s+(\d+)\s+ROWS\s+ONLY" , @"(?i)SELECT\s+TOP\s+(\d+)" };
      
        public override string RemovePagination(string sql)
        {
            var patternOffsetFetch = @"(?i)OFFSET\s+(\d+)\s+ROWS(?:\s+FETCH\s+NEXT\s+(\d+)\s+ROWS\s+ONLY)?";
            sql = Regex.Replace(sql, patternOffsetFetch, "");

            var patternTop = @"(?i)SELECT\s+TOP\s+(\d+)\s+";
            sql = Regex.Replace(sql, patternTop, "SELECT ");

            return sql;
        }
        public override StringBuilder GetTableColumnsSql(String dbName, String tableName, string schemaName = "public")
        {
            return new StringBuilder(@$"SELECT 
    COLUMN_NAME AS ColName,
    DATA_TYPE AS DataType,
    CHARACTER_MAXIMUM_LENGTH AS DataLength,
    NUMERIC_PRECISION AS DataPrecision,
    NUMERIC_SCALE AS DataScale,
    CASE 
        WHEN COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 
            OR EXISTS (
                SELECT 1 
                FROM sys.index_columns ic
                JOIN sys.indexes i ON ic.index_id = i.index_id AND ic.object_id = i.object_id
                WHERE ic.object_id = OBJECT_ID(TABLE_SCHEMA + '.' + TABLE_NAME)
                    AND ic.column_id = COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'ColumnId')
                    AND i.is_primary_key = 1
            ) 
        THEN 1 
        ELSE 0 
    END AS ColKey,
    CASE WHEN IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS Nullable,
    ORDINAL_POSITION AS ColPosition,
    COLUMN_DEFAULT AS DataDefault,
    CAST(value AS NVARCHAR(MAX)) AS ColComment
FROM 
    INFORMATION_SCHEMA.COLUMNS
LEFT JOIN 
    sys.extended_properties 
ON 
    sys.extended_properties.major_id = OBJECT_ID(QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME))
    AND sys.extended_properties.minor_id = COLUMNPROPERTY(OBJECT_ID(QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME)), COLUMN_NAME, 'ColumnId')
WHERE 
    TABLE_CATALOG = '{dbName}'
    AND TABLE_NAME = '{tableName}';");
        }

        public override StringBuilder GetTablesSql(String dbName, string schemaName = "public")
        {
            return new StringBuilder(@$"SELECT 
    TABLE_NAME AS TableName, 
    CAST(value AS NVARCHAR(MAX)) AS TableComment
FROM 
    INFORMATION_SCHEMA.TABLES
LEFT JOIN 
    sys.extended_properties 
ON 
    sys.extended_properties.major_id = OBJECT_ID(QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME))
    AND sys.extended_properties.minor_id = 0
WHERE TABLE_CATALOG = '{dbName}';");
        }
    }


}