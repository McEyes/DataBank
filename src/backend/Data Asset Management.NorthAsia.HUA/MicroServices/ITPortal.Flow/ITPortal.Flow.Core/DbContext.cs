using Furion;

using ITPortal.Core.Repositorys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SqlSugar;

using System;
using System.Collections.Generic;

namespace ITPortal.Flow.Core
{
    ///// <summary>
    ///// 数据库上下文对象
    ///// </summary>
    //public static class DbContext
    //{
    //    /// <summary>
    //    /// SqlSugar 数据库实例
    //    /// </summary>
    //    public static readonly SqlSugarScope Instance = new(
    //        // 读取 appsettings.json 中的 ConnectionConfigs 配置节点
    //        App.GetConfig<List<ConnectionConfig>>("ConnectionConfigs")
    //        , db =>
    //        {
    //            // 这里配置全局事件，比如拦截执行 SQL
    //        });
    //}
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services, IConfiguration configuration, string dbName = "ITPortalFlow")
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

            SqlSugarScope sqlSugar = new SqlSugarScope(configConnection,
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
            services.AddSingleton<ISqlSugarClient>(sqlSugar);//这边是SqlSugarScope用AddSingleton
        }
    }
}
