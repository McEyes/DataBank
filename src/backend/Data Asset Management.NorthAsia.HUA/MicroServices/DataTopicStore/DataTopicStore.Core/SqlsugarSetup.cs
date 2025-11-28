using System;
using System.Linq;
using DataTopicStore.Core.Repositories;
using Furion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace DataTopicStore.Core
{
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            // 如果多个数数据库传 List<ConnectionConfig>
            var configConnection = new ConnectionConfig()
            {
                DbType = DbType.PostgreSQL,
                ConnectionString = App.Configuration.GetConnectionString("DataTopicStore"),
                IsAutoCloseConnection = true,
            };

            SqlSugarScope sqlSugar = new SqlSugarScope(configConnection,
            db =>
            {
                // 单例参数配置，所有上下文生效
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine(sql);//输出sql
                    Console.WriteLine(string.Join(",",pars.Select(t=>$"{t.ParameterName}={t.Value}")));//输出sql
                };
            });
           
            services.AddSingleton<ISqlSugarClient>(sqlSugar);   // 这边是SqlSugarScope用AddSingleton
            services.AddScoped(typeof(Repository<>));
        }
    }
}
