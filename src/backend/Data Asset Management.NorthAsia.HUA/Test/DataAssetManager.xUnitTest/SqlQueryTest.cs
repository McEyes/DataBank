using DataAssetManager.DataApiServer.Application;
using DataAssetManager.DataTableServer.Application;

using Elastic.Clients.Elasticsearch.Ingest;

using Furion;
using Furion.FriendlyException;

using ITPortal.Core.DataSource;
using ITPortal.Core.SqlParse;
using ITPortal.Extension.System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;

using SqlSugar;

using StackExchange.Profiling.Internal;

using System.Diagnostics;

using System.Xml;

using Xunit.Abstractions;

namespace DataAssetManager.Test
{
    public class SqlQueryTest
    {
        // 创建服务集合
        private readonly ServiceCollection services = new ServiceCollection();
        private readonly IDataApiService _apiservcie;
        private readonly IDataSourceService _dataSourceServcie;
        private readonly CacheDbQueryFactory _cacheDbQueryFactory;
        private readonly ITestOutputHelper Output;


        /// <summary>
        /// 这里不能通过构造函数注入，而是采用 App.GetService<> 方式
        /// </summary>
        public SqlQueryTest(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
            _apiservcie = App.GetService<IDataApiService>();
            _dataSourceServcie = App.GetService<IDataSourceService>();
            _cacheDbQueryFactory = App.GetService<CacheDbQueryFactory>();
        }

        [Fact]
        public async Task TestMethod1()
        {
            var list = _apiservcie.AsQueryable().OrderByDescending(f=>f.CreateTime)
                                        .Take(10).ToList();

            foreach (var apiEntiy in list)
            {
                try
                {
                    if(apiEntiy.ApiName== "asset_user")
                    {

                    }

                    var datasource = await _dataSourceServcie.Get(apiEntiy.SourceId);
                    var dbtype = DbSchema.GetDbType(datasource.DbType);
                    ISqlParser sqlparser = SqlParserFactory.CreateParser(dbtype);

                    #region sql执行
                    var sw = new Stopwatch();
                    sw.Start();
                    datasource.DbSchema.Host = "10.114.17.230";
                    datasource.DbSchema.Username = "performancetestuser";
                    datasource.DbSchema.Password = "performancetestuser";
                    var db = _cacheDbQueryFactory.CreateSqlClient(datasource.DbSchema);
                    var totalnumber = new RefAsync<int>();
                    List<object> result = null;
                    try
                    {
                        //检查字段是否包含,并过滤不包含的字段
                        var sqlText = apiEntiy.ExecuteConfig.sqlText;

                        //替换条件
                        sqlText = sqlparser.ReplaceConditions(apiEntiy.ExecuteConfig.sqlText, new Dictionary<string, object>());

                        var query = db.SqlQueryable<object>(sqlText);
                        result = await query.ToPageListAsync(1, 10, totalnumber);
                        Output.WriteLine($"执行{apiEntiy.ApiName}耗时：{sw.ElapsedMilliseconds},获取结果：{result?.Count()}");
                        Output.WriteLine(result.ToJSON());
                    }
                    catch (Exception ex)
                    {
                        Output.WriteLine($"查询sql异常,当前数据库：{db.CurrentConnectionConfig.ConnectionString} \r\n{ex.Message}\r\n{ex.StackTrace}");
                        throw new AppFriendlyException(ex.Message, ex);
                    }
                    #endregion sql执行
                    sw.Stop();
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"执行{apiEntiy.ApiName}异常：{ex.StackTrace}");
                }
                Output.WriteLine("");
            }
        }


    }
}

