using Elastic.Clients.Elasticsearch.Xpack;

using Furion;
using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.Extensions;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.Repositorys;
using ITPortal.Core.Services;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SqlSugar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    db.Aop.OnDiffLogEvent = async it =>
                    {
                        var CurrentUser = App.HttpContext.GetCurrUserInfo() ?? new UserInfo();
                        var list = GetDiff(it.BeforeData, it.AfterData, CurrentUser.UserName);
                        var result = await db.CopyNew().Insertable(list).ExecuteCommandAsync();
                        Console.WriteLine($"{it.DiffType} {list.Count}");
                        //var serviceBuilder = services.BuildServiceProvider();
                        //var eventPublisher = serviceBuilder.GetService<IEventPublisher>();
                        //var _ = eventPublisher.PublishAsync(new ElasticEventSource(DataAssetManagerConst.DataChangeRecordEvent, typeof(List<DataChangeRecordEntity>), list));
                    };
                    db.Aop.DataExecuting = (oldValue, entityInfo) =>
                    {
                        /*** 列级别事件：插入的每个列都会进事件 ***/
                        if (entityInfo.OperationType == DataFilterType.InsertByObject)//entityInfo.PropertyName == "CreateTime" && 
                        {
                            //entityInfo.SetValue(DateTime.Now);//修改CreateTime字段

                            /*entityInfo有字段所有参数*/

                            /*oldValue表示当前字段值 等同于下面写法*/
                            //var value=entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue);

                            /*获取当前列特性*/
                            //5.1.3.23 +
                            //entityInfo.IsAnyAttribute<特性>()
                            //entityInfo.GetAttribute<特性>()
                        }
                        else if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                        {
                            //entityInfo.SetValue(DateTime.Now);//修改CreateTime字段

                            /*entityInfo有字段所有参数*/

                            /*oldValue表示当前字段值 等同于下面写法*/
                            //var value=entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue);

                            /*获取当前列特性*/
                            //5.1.3.23 +
                            //entityInfo.IsAnyAttribute<特性>()
                            //entityInfo.GetAttribute<特性>()
                        }
                        else if (entityInfo.OperationType == DataFilterType.DeleteByObject)
                        {/*** 删除生效 （只有行级事件） ***/
                            //entityInfo.EntityValue 拿到所有实体对象
                        }


                        /*** 行级别事件 ：一条记录只会进一次 ***/
                        if (entityInfo.EntityColumnInfo.IsPrimarykey)
                        {
                            //entityInfo.EntityValue 拿到单条实体对象
                        }

                    };

                    db.Aop.OnError = (exp) =>//SQL报错
                    {
                        var serviceBuilder = services.BuildServiceProvider();
                        var log = serviceBuilder.GetService<ILogger<SqlSugarScope>>();
                        //获取无参数化SQL 影响性能只适合调试
                        //UtilMethods.GetSqlString(DbType.SqlServer,sql,pars)
                        //UtilMethods.GetSqlString(DbType.TDSQLForPGODBC, sql, pars);
                        //获取原生SQL推荐 5.1.4.63  性能OK
                        log?.LogError(UtilMethods.GetNativeSql(exp.Sql, exp.Parametres as SugarParameter[]));//输出sql   
                    };
#if DEBUG || STG
                    //单例参数配置，所有上下文生效
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        //var serviceBuilder = services.BuildServiceProvider();
                        //var log = serviceBuilder.GetService<ILogger<SqlSugarScope>>();
                        //获取无参数化SQL 影响性能只适合调试
                        //UtilMethods.GetSqlString(DbType.SqlServer,sql,pars)
                        //UtilMethods.GetSqlString(DbType.TDSQLForPGODBC, sql, pars);
                        //获取原生SQL推荐 5.1.4.63  性能OK
                        //log?.LogInformation(UtilMethods.GetNativeSql(sql, pars));//输出sql
                        //log.LogTrace(string.Join(",", pars.Select(t => $"{t.ParameterName}={t.Value}")));//输出sql
                    };
#endif
                });

            services.AddScoped(typeof(Repository<>));
            services.AddSingleton<ISqlSugarClient>(sqlSugar);//这边是SqlSugarScope用AddSingleton
        }

        public static List<string> IgnoreColumns = new List<string>()
      {
         "update_time","update_by","create_time","create_by","create_by_name","update_by_name"// "CreateTime","CreateBy", "UpdateTime","UpdateBy", "CreatedByName","UpdateByName",
      };
        /// <summary>
        /// 比较两个数据对象的修改内容
        /// </summary>
        /// <param name="beforeData"></param>
        /// <param name="afterData"></param>
        /// <returns></returns>
        public static List<DataChangeRecordEntity> GetDiff(List<DiffLogTableInfo> beforeData, List<DiffLogTableInfo> afterData, string userName)
        {
            string mianID = null;
            var curDate = DateTime.Now;
            if (beforeData != null && beforeData.Count > 0)
            {
                var keyCoulumn = beforeData[0].Columns.FirstOrDefault(p => p.IsPrimaryKey == true);
                if (keyCoulumn == null)
                    keyCoulumn = beforeData[0].Columns.FirstOrDefault(p => p.ColumnName.Equals("id", StringComparison.CurrentCultureIgnoreCase));
                if (keyCoulumn != null)
                {
                    mianID = keyCoulumn.Value.ToString();
                }
            }
            else if (afterData != null && afterData.Count > 0)
            {
                var keyCoulumn = afterData[0].Columns.FirstOrDefault(p => p.IsPrimaryKey == true);
                if (keyCoulumn == null)
                    keyCoulumn = afterData[0].Columns.FirstOrDefault(p => p.ColumnName.Equals("id", StringComparison.CurrentCultureIgnoreCase));
                if (keyCoulumn != null)
                {
                    mianID = keyCoulumn.Value.ToString();
                }
            }
            var changeList = new List<DataChangeRecordEntity>();
            if (beforeData != null && afterData != null)
            {
                var befroeColumns = beforeData[0].Columns;
                var afterCloums = afterData[0].Columns;
                foreach (var item in afterCloums)
                {
                    if (IgnoreColumns.Contains(item.ColumnName.ToLower()))
                        continue;
                    var before = befroeColumns.FirstOrDefault(p => p.ColumnName == item.ColumnName && !p.Value.Equals(item.Value));
                    if (before == null) continue;
                    changeList.Add(new DataChangeRecordEntity()
                    {
                        Id = SnowflakeIdGenerator.NextUid(20), //Guid.NewGuid().ToString(),
                        ObjectId = mianID,
                        ObjectType = beforeData[0].TableName,
                        FieldName = item.ColumnName,
                        FieldOldValue = before.Value.ToString(),
                        FieldNewValue = item.Value.ToString(),
                        Remark = $"update {beforeData[0].TableName}.{item.ColumnName}={item.Value}",
                        CreateBy = userName,
                        CreateTime = curDate,
                        Status = 2
                    });
                }
            }
            else if (beforeData != null && beforeData.Count > 0)
            {
                var befroeColumns = beforeData[0].Columns;
                foreach (var item in befroeColumns)
                {
                    //if (IgnoreColumns.Contains(item.ColumnName.ToLower()))
                    //    continue;
                    changeList.Add(new DataChangeRecordEntity()
                    {
                        Id = SnowflakeIdGenerator.NextUid(20), //Guid.NewGuid().ToString(),
                        ObjectId = mianID,
                        ObjectType = beforeData[0].TableName,
                        FieldName = item.ColumnName,
                        FieldOldValue = item.Value.ToString(),
                        Remark = $"delete {beforeData[0].TableName}.{item.ColumnName}={item.Value}",
                        CreateBy = userName,
                        CreateTime = curDate,
                        Status = 3
                    });
                }
            }
            else if (afterData != null && afterData.Count > 0)
            {
                var afterCloums = afterData[0].Columns;
                foreach (var item in afterCloums)
                {
                    if (IgnoreColumns.Contains(item.ColumnName.ToLower()))
                        continue;
                    changeList.Add(new DataChangeRecordEntity()
                    {
                        Id = SnowflakeIdGenerator.NextUid(20), // Guid.NewGuid().ToString(),
                        ObjectId = mianID,
                        ObjectType = afterData[0].TableName,
                        FieldName = item.ColumnName,
                        FieldNewValue = item.Value.ToString(),
                        Remark = $"insert {afterData[0].TableName}.{item.ColumnName}={item.Value}",
                        CreateBy = userName,
                        CreateTime = curDate,
                        Status = 1
                    });
                }
            }
            return changeList;
        }
    }
}
