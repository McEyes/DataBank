using Furion;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SqlSugar;

using System;
using System.Collections.Generic;

namespace ITPortal.AuthServer.Web.Core
{
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services, IConfiguration configuration, string dbName = "JabilService")
        {
            //如果多个数数据库传 List<ConnectionConfig>
            var configConnection = new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.PostgreSQL,
                ConnectionString = configuration.GetConnectionString(dbName),
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings()
                {
                    IsWithNoLockQuery = true,
                    PgSqlIsAutoToLower = false,
                    PgSqlIsAutoToLowerCodeFirst = false
                }
            };

            SqlSugarScope sqlSugar = new SqlSugarScope(configConnection,
                db =>
                {
                    //单例参数配置，所有上下文生效
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        Console.WriteLine(sql);//输出sql
                    };
                });

            services.AddSingleton<ISqlSugarClient>(sqlSugar);//这边是SqlSugarScope用AddSingleton
        }
    }
}
