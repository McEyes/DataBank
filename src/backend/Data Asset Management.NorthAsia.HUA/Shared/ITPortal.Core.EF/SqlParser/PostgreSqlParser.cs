using System.Data;
using System.Text;

namespace ITPortal.Core.SqlParse
{
    // PostgreSQL ������ʵ��
    public class PostgreSqlParser : SqlParser
    {
        public PostgreSqlParser(DbType sugerDbType) : base(sugerDbType)
        {
        }

        public override StringBuilder GetTableColumnsSql(String dbName, String tableName, string schemaName = "public")
        {
            return new StringBuilder(@$"SELECT 
    column_name AS ColName,
    data_type AS DataType,
    character_maximum_length AS DataLength,
    numeric_precision AS DataPrecision,
    numeric_scale AS DataScale,
    EXISTS (
        SELECT 1 
        FROM pg_index i
        JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = ANY(i.indkey)
        WHERE i.indrelid = format('%I.%I', '{schemaName}', '{tableName}')::regclass
            AND a.attname = column_name
            AND i.indisprimary
    ) AS ColKey,
    is_nullable = 'YES' AS Nullable,
    ordinal_position AS ColPosition,
    column_default AS DataDefault,
    col_description(format('%I.%I', '{schemaName}', '{tableName}')::regclass::oid, ordinal_position) AS ColComment
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    table_schema = '{schemaName}'
    AND table_name = '{tableName}';");
        }

        public override StringBuilder GetTablesSql(String dbName,string schemaName= "public")
        {
            return new StringBuilder(@$"SELECT 
    tablename, 
    obj_description(pg_class.oid, 'pg_class') AS TABLE_COMMENT
FROM 
    pg_tables
JOIN 
    pg_class ON pg_tables.tablename = pg_class.relname
WHERE 
    schemaname = '{schemaName}';");
        }

    }
}