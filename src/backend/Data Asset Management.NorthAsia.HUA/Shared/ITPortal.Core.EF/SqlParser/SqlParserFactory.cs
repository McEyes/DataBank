using ITPortal.Core.Exceptions;



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.SqlParse
{
    public class SqlParserFactory
    {
        //public static ISqlParser CreateParser(int? dbType)
        //{
        //    var databaseType=string.Empty;
        //    //if (int.TryParse(databaseType, out int dbType))
        //    if (dbType.HasValue)
        //    {
        //        databaseType = ((DataSource.DbType)dbType).ToString();
        //    }
        //    switch (databaseType.ToUpper())
        //    {
        //        case "MYSQL":
        //        case "MARIADB":
        //            return new MySqlParser(DbType.MySql);
        //        case "SQLSERVER":
        //        case "SQL_SERVER":
        //            return new SqlServerParser(DbType.SqlServer);
        //        case "ORACLE":
        //            return new OracleParser(DbType.Oracle);
        //        case "POSTGRESQL":
        //        case "POSTGRE_SQL":
        //            return new PostgreSqlParser(DbType.PostgreSQL);
        //        case "SQLITE":
        //            return new SqliteParser(DbType.Sqlite);
        //        default:
        //            throw new ArgumentException($"{databaseType} Unsupported database type");
        //    }
        //}

        //public static ISqlParser CreateParser(ISqlSugarClient db)
        //{
        //    var type = db.CurrentConnectionConfig.DbType;
        //    switch (type)
        //    {
        //        case SqlSugar.DbType.MySql:
        //            return new MySqlParser(DbType.MySql);
        //        case SqlSugar.DbType.SqlServer:
        //            return new SqlServerParser(DbType.SqlServer);
        //        case SqlSugar.DbType.PostgreSQL:
        //            return new PostgreSqlParser(DbType.PostgreSQL);
        //        case SqlSugar.DbType.Oracle:
        //            return new OracleParser(DbType.Oracle);
        //        case SqlSugar.DbType.Sqlite:
        //            return new SqliteParser(DbType.Sqlite);
        //    }
        //    throw new DataQueryException("不支持的数据库类型");
        //}
        //public static ISqlParser CreateParser(ITPortal.Core.DataSource.DbType type)
        //{
        //    switch (type)
        //    {
        //        case DataSource.DbType.MYSQL:
        //            return new MySqlParser(DbType.MySql);
        //        case DataSource.DbType.SQL_SERVER:
        //            return new SqlServerParser(DbType.SqlServer);
        //        case DataSource.DbType.POSTGRE_SQL:
        //            return new PostgreSqlParser(DbType.PostgreSQL);
        //        case DataSource.DbType.ORACLE:
        //            return new OracleParser(DbType.Oracle);
        //        case DataSource.DbType.Sqlite:
        //            return new SqliteParser(DbType.Sqlite);
        //    }
        //    throw new DataQueryException("不支持的数据库类型");
        //}
    }

}
