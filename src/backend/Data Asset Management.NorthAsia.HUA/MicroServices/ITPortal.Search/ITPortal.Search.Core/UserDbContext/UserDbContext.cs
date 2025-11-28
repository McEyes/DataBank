using Furion;

using ITPortal.Core.Repositorys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SqlSugar;

using System;
using System.Collections.Generic;

namespace ITPortal.Search.Core
{
    public static class UserSqlsugarSetup
    {
        public static void AddUserSqlsugarSetup(this IServiceCollection services, IConfiguration configuration, string dbName = "JabilService")
        {
            //如果多个数数据库传 List<ConnectionConfig>
            var configConnection = new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.PostgreSQL,
                ConnectionString = configuration.GetConnectionString(dbName),
                IsAutoCloseConnection = true,
            };
            if (configConnection.MoreSettings == null)
                configConnection.MoreSettings = new ConnMoreSettings();
            configConnection.MoreSettings.PgSqlIsAutoToLower = false;

            UserDbSqlSugarScope sqlSugar = new UserDbSqlSugarScope(configConnection,
                db =>
                {
#if DEBUG || STG
                    //单例参数配置，所有上下文生效
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        Console.WriteLine(sql);//输出sql
                    };
#endif
                });

            services.AddScoped(typeof(Repository<>));
            services.AddSingleton<IUserDbSugarClient>(sqlSugar);//这边是SqlSugarScope用AddSingleton
        }
    }
}
