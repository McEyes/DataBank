using ITPortal.Core.Services;

using Furion.JsonSerialization;

using Microsoft.Extensions.Caching.Memory;

using SqlSugar;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ITPortal.Core.DataSource
{

    public class CacheDbQueryFactory : IDataSourceFactory
    {
        private readonly IMemoryCache _cache;
        //private static readonly ConcurrentDictionary<string, ISqlSugarClient> _cache = new ConcurrentDictionary<string, ISqlSugarClient>();
        public CacheDbQueryFactory(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 创建数据源实例
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public ISqlSugarClient CreateSqlClient(DbSchema property)
        {
            if (_cache.TryGetValue(property.ToString(), out ISqlSugarClient? sqlSugar))
            {
                sqlSugar.ClearTracking();
                Console.WriteLine("from cache get sqlsugar......");//输出sql
                return sqlSugar;
            }
            var configConnection = new ConnectionConfig()
            {
                DbType = property.GetSugarDbType(),
                ConnectionString = property.GetConnectionString(),
                ConfigId = property.ToString(),
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings()
                {
                    IsWithNoLockQuery = true,
                }
            };

            sqlSugar = new SqlSugarScope(configConnection,
                db =>
                {
                    //单例参数配置，所有上下文生效
                    //Console.WriteLine(db.CurrentConnectionConfig.ConnectionString);

                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        //Console.WriteLine(sql);//输出sql
                        Console.WriteLine($"\t\t执行sql：\r\n\t{sql}\r\n\t\t参数：{JSON.Serialize(pars)}");//输出sql
                    };
                });
            _cache.Set(property.ToString(), sqlSugar);
            return sqlSugar;
        }
    }

}
