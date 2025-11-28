using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.DataSource
{
    public enum DbType
    { 
       
        /// <summary>
        /// MySql DB
        /// </summary>
        [Description()]
        MYSQL = 1,//("1", "MySql DB", "jdbc:mysql://${host}:${port}/${dbName}?useUnicode=true&characterEncoding=utf-8&zeroDateTimeBehavior=convertToNull&useSSL=false&serverTimezone=GMT%2B8"),
        /// <summary>
        /// OracleDB
        /// </summary>
        ORACLE = 4,//("4", "OracleDB", "jdbc:oracle:thin:@${host}:${port}:${sid}"),
        /// <summary>
        /// PostgreSql DB
        /// </summary>
        POSTGRE_SQL = 5,//("5", "PostgreSql DB", "jdbc:postgresql://${host}:${port}/${dbName}"),
        /// <summary>
        /// SQLServer DB
        /// </summary>
        SQL_SERVER = 7,//("7", "SQLServer DB", "jdbc:sqlserver://${host}:${port};DatabaseName=${dbName}"),
        /// <summary>
        /// UNKONWN DB
        /// </summary>
        OTHER = 8,//("8", "other DB", "");
        /// <summary>
        /// Sqlite
        /// </summary>
        Sqlite = 9,//("8", "other DB", "");
    }
}
