using ITPortal.Core.Exceptions;

using Furion.Templates;
using Microsoft.Extensions.Hosting;
using SqlSugar.DistributedSystem.Snowflake;

using StackExchange.Profiling.Internal;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;
using ITPortal.Core.Extensions;

namespace ITPortal.Core.DataSource
{
    public class DbSchema
    {
        private Dictionary<string, string> hostNameMapIP => new Dictionary<string, string>(){
            { "huam0itstg87", "10.114.17.125" },
            { "cnhuam0itpoc90","10.114.26.216" },
            { "huam0itstg88","10.114.20.252" }
        };

        /// <summary>
        /// 数据库类型
        /// </summary>
        public int? Dbtype { get; set; }
        /// <summary>
        /// 数据库连接地址
        /// </summary>
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string DbName { get; set; }
        public string Sid { get; set; }

        public string GetConnectionString()
        {
            var dbType = GetDbType(Dbtype);
            var HostIp = Host;
            hostNameMapIP.TryGetValue(Host, out HostIp);
            switch (dbType)
            {
                case DbType.MYSQL:
                    //server=localhost;Database=SqlSugar4xTest;Uid=root;Pwd=haosql;TrustServerCertificate=true ;Pooling=false;Pooling=false;// ;Pooling=false;  自动释放
                    //--default-auth=mysql_native_password
                    //server=fe1,fe2,fe3;port=9030;database=MP;user=root;password='hjq@123';Pooling=true;LoadBalance=RoundRobin;
                    var connStr = $"server={HostIp ?? Host};Port={Port};Database={DbName};Uid={Username};Pwd={Password};Pooling=false;";
                    //doris连接
                    //多数情况下需要禁用连接 Pooling=false
                    //ConnectionString = "server=localhost;Database=SqlSugar4xTest;Uid=root;Pwd=haosql;Pooling=false"
                    //connStr = "server=10.114.17.230;Port=9030;Database=Trace_SCC;Uid=performancetestuser;Pwd=performancetestuser;Pooling=false";
                    return connStr;
                case DbType.ORACLE:
                    // 写法1
                    //  Data Source=localhost/orcl;User ID=system;Password=haha
                    // 字法2 上面连不上可以试用下面写法
                    // Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=150.158.57.125)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id=xx;Password=xx;Pooling='true';Max Pool Size=150
                    return $"Data Source={HostIp ?? Host}/orcl;User ID={Username};Password={Password}";
                    //return $"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={HostIp ?? Host})(PORT={Port})))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id={Username};Password={Password};Pooling='true';Max Pool Size=150";
                case DbType.POSTGRE_SQL:
                    // PORT=5432;DATABASE=SqlSugar4xTest;HOST=localhost;PASSWORD=haosql;USER ID=postgres
                    return $"PORT={Port};DATABASE={DbName};HOST={Host};PASSWORD={Password};USER ID={Username}";
                case DbType.SQL_SERVER:
                    //server=.;uid=sa;pwd=haosql;database=SQLSUGAR4XTEST;Encrypt=True;TrustServerCertificate=True;
                    return $"server={HostIp ?? Host},{Port};uid={Username};pwd={Password};database={DbName};Encrypt=True;TrustServerCertificate=True;";
                default:
                case DbType.OTHER:
                    return string.Empty;
            };
        }

        public SqlSugar.DbType GetSugarDbType()
        {
            var dbType = GetDbType(Dbtype);
            switch (dbType)
            {
                case DbType.MYSQL:
                    return SqlSugar.DbType.MySql;
                case DbType.ORACLE:
                    return SqlSugar.DbType.Oracle;
                case DbType.POSTGRE_SQL:
                    return SqlSugar.DbType.PostgreSQL;
                case DbType.SQL_SERVER:
                    return SqlSugar.DbType.SqlServer;
                default:
                case DbType.OTHER:
                    return SqlSugar.DbType.Custom;
            };
        }

        public override string ToString()
        {
            return Furion.DataEncryption.MD5Encryption.Encrypt($"{Dbtype}:{Host}:{Port}:{DbName}:{Username}:{Password}:{Sid};", is16: true);
        }

        public void viald()
        {
            if (!Dbtype.HasValue || Host.IsNullOrWhiteSpace()
                || Username.IsNullOrWhiteSpace() || Password.IsNullOrWhiteSpace()
                 || DbName.IsNullOrWhiteSpace() || Port == 0)
            {
                throw new DataQueryException("Incomplete parameters");
            }
            if (DbType.OTHER == GetDbType(Dbtype))
            {
                throw new DataQueryException("Unsupported database type");
            }
        }

        public static DbType GetDbType(int? dbType)
        {
            foreach (DbType type in Enum.GetValues(typeof(DbType)))
            {
                if (type.ToInt() == dbType)
                {
                    return type;
                }
            }
            return DbType.OTHER;
        }
    }
}
