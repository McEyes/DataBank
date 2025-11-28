using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Core.Models
{
    public class DbSettings
    {
        public DbSettings() { }
        public DbSettings(string host, int port, string dbName, string userName, string password, string tableId, string tableName, int dbType)
        {
            this.Host = host;
            this.Port = port;
            this.DbName = dbName;
            this.Username = userName;
            this.Password = password;
            this.TableName = tableName;
            this.TableId = tableId;
            switch (dbType)
            {
                case 1:
                    this.DbType = DatabaseType.Mysql;
                    break;
                case 5:
                    this.DbType = DatabaseType.Postgresql;
                    break;
                case 7:
                    this.DbType = DatabaseType.SqlServer;
                    break;
                default:
                    this.DbType = DatabaseType.Postgresql;
                    break;
            }
        }

        

        public string Host { get; set; }
        public int Port { get; set; }
        public string DbName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TableName { get; set; }
        public string TableId { get; set; }
        public DatabaseType DbType { get; set; }
    }
}
