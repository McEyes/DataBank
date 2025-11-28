using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace ITPortal.Core.SqlParse
{
    // Oracle ������ʵ��
    public class OracleParser : SqlParser
    {
        public OracleParser(DbType sugerDbType) : base(sugerDbType)
        {
        }

        public override string[] PagePattern => new string[] { @"(?i)(WHERE\s+ROWNUM\s*(<|<=)\s*(\d+))|(AND\s+ROWNUM\s*(<|<=)\s*(\d+))", @"(?i)OFFSET\s+(\d+)\s+ROWS\s+(FETCH\s+(FIRST|NEXT)\s+(\d+)\s+ROWS\s+ONLY)" };


        public override StringBuilder GetTableColumnsSql(String dbName, String tableName, string schemaName = "public")
        {
            return new StringBuilder(@$"SELECT 
    COLUMN_NAME AS ColName,
    DATA_TYPE AS DataType,
    DATA_LENGTH AS DataLength,
    DATA_PRECISION AS DataPrecision,
    DATA_SCALE AS DataScale,
    EXISTS (
        SELECT 1 
        FROM ALL_CONS_COLUMNS cc
        JOIN ALL_CONSTRAINTS c ON cc.CONSTRAINT_NAME = c.CONSTRAINT_NAME AND cc.OWNER = c.OWNER
        WHERE cc.TABLE_NAME = '{tableName}'
            AND cc.COLUMN_NAME = cols.COLUMN_NAME
            AND c.CONSTRAINT_TYPE = 'P'
    ) AS ColKey,
    NULLABLE = 'Y' AS Nullable,
    COLUMN_ID AS ColPosition,
    DATA_DEFAULT AS DataDefault,
    COMMENTS AS ColComment
FROM 
    ALL_TAB_COLUMNS cols
LEFT JOIN 
    ALL_COL_COMMENTS comms 
ON 
    cols.TABLE_NAME = comms.TABLE_NAME
    AND cols.COLUMN_NAME = comms.COLUMN_NAME
WHERE 
    cols.OWNER = '{dbName}'
    AND cols.TABLE_NAME = '{tableName}';");
        }

        public override StringBuilder GetTablesSql(String dbName, string schemaName = "public")
        {
            return new StringBuilder(@$"SELECT 
    TABLE_NAME AS TableName, 
    COMMENTS AS TableComment
FROM 
    ALL_TAB_COMMENTS
WHERE 
    OWNER = '{dbName}';");
        }

    }
}