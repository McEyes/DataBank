using Furion;

using ITPortal.Core.Repositorys;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SqlSugar;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAssetManager.DataApiServer.Core
{
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services, IConfiguration configuration, string dbName = "DataxAccess")
        {
            //如果多个数数据库传 List<ConnectionConfig>
            var configConnection = new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.PostgreSQL,
                ConnectionString = configuration.GetConnectionString(dbName),
                //MoreSettings = new ConnMoreSettings() { PgSqlIsAutoToLower = false },
                IsAutoCloseConnection = true,
            };

            SqlSugarScope sqlSugar = new SqlSugarScope(configConnection,
                db =>
                {
#if DEBUG || STG
                    //单例参数配置，所有上下文生效
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        Console.WriteLine(sql);//输出sql
                        Console.WriteLine(string.Join(",", pars.Select(t => $"{t.ParameterName}={t.Value}")));//输出sql
                    };
#endif
                });

            services.AddScoped(typeof(Repository<>));
            services.AddSingleton<ISqlSugarClient>(sqlSugar);//这边是SqlSugarScope用AddSingleton
        }
    }
}
