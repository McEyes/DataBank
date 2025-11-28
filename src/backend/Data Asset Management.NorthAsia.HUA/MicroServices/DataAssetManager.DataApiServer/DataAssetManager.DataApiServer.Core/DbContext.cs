using Furion;

using SqlSugar;

using System;
using System.Collections.Generic;

namespace DataAssetManager.DataApiServer.Core
{
    /// <summary>
    /// 数据库上下文对象
    /// </summary>
    public static class DbContext
    {
        /// <summary>
        /// SqlSugar 数据库实例
        /// </summary>
        public static readonly SqlSugarScope Instance = new SqlSugarScope(
            // 读取 appsettings.json 中的 ConnectionConfigs 配置节点
            App.GetConfig<List<ConnectionConfig>>("ConnectionConfigs")
            , db =>
            {
                // 这里配置全局事件，比如拦截执行 SQL
                Console.WriteLine($"执行 ConnectionString：{db.Context.CurrentConnectionConfig.ConnectionString}");
                //Console.WriteLine($"执行 SQL：{db.}");
                //调试SQL事件，可以删掉
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //获取原生SQL推荐 5.1.4.63  性能OK
                    Console.WriteLine("原生SQL:" + UtilMethods.GetNativeSql(sql, pars));
                    //获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
                    Console.WriteLine("无参SQL:" + UtilMethods.GetSqlString(DbType.SqlServer, sql, pars));
                };
            });
    }
}
