using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace DataTopicStore.DbMigrator
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var db = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = DbType.PostgreSQL, // Specify PostgreSQL as the database type
                ConnectionString = "Host=cnhuam0itds01;Port=5432;Database=DataTopicStore;Username=******;Password=password;",
                IsAutoCloseConnection = true // Automatically close the connection
            });

            db.DbFirst.IsCreateAttribute().CreateClassFile("C:\\temp\\models", "DataTopicStore.Core.Entities");
        }
    }
}
