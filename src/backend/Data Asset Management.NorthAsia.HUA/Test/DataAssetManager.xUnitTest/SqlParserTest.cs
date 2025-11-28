using DataAssetManager.DataApiServer.Application;
using DataAssetManager.DataTableServer.Application;

using Furion;

using ITPortal.Core.DataSource;
using ITPortal.Core.SqlParse;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;

using SqlSugar;

using StackExchange.Profiling.Internal;

using Xunit.Abstractions;

namespace DataAssetManager.Test
{
    public  class SqlParserTest
    {
        // 创建服务集合
        static ServiceCollection services = new ServiceCollection();
        private static IDataApiLogService _apiLogService;
        private static IDataApiService _apiservcie;
        private static IDataSourceService _dataSourceServcie;
        private readonly ITestOutputHelper Output;


        /// <summary>
        /// 这里不能通过构造函数注入，而是采用 App.GetService<> 方式
        /// </summary>
        public SqlParserTest(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
            _apiLogService = App.GetService<IDataApiLogService>();
            _apiservcie = App.GetService<IDataApiService>();
            _dataSourceServcie = App.GetService<IDataSourceService>();
        }

        [Fact]
        public async Task TestMethod1()
        {
            var list = _apiLogService.AsQueryable().OrderByDescending(f => f.CallerDate)
                                        .Take(100).ToList();


            foreach (var item in list)
            {
                try
                {
                    var apiEntiy = await _apiservcie.Get(item.ApiId);
                    var datasource = await _dataSourceServcie.Get(apiEntiy.SourceId);
                    var dbtype = DbSchema.GetDbType(datasource.DbType);
                    ISqlParser sqlparser = SqlParserFactory.CreateParser(dbtype);
                    if (item.CallerParam.sqlText.IsNullOrWhiteSpace())
                    {
                        Console.WriteLine($"param:\t\t\t{item.CallerParams}");
                        continue;
                    }

                    Console.WriteLine("\n\n");
                    Console.WriteLine($"Db:\t\t\t\t{dbtype}");
                    Console.WriteLine($"DBConn:\t\t\t{datasource.DbSchema.GetConnectionString()}");
                    Console.WriteLine($"sql:\t\t\t{item.CallerParam.sqlText}");
                    Console.WriteLine($"tableNames:\t\t{string.Join(',', sqlparser.ExtractTableNames(item.CallerParam.sqlText))}");
                    Console.WriteLine($"PageInfo:\t\t{sqlparser.ExtractPaginationInfo(item.CallerParam.sqlText)}");
                    Console.WriteLine($"GetSqlType:\t\t{sqlparser.GetSqlType(item.CallerParam.sqlText)}");
                    var pageInfo = sqlparser.ExtractPaginationInfo(item.CallerParam.sqlText);
                    Console.WriteLine($"paginationInfo:\t{pageInfo.paginationInfo}");
                    Console.WriteLine("\n\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"param:\t\t\t{item.CallerParams}");
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 测试from的sql字段解析
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestFromSqlParser()
        {
            var list = await _apiservcie.AllFromCache();
            var apiList = list.Where(f => f.ApiUrl.EndsWith("/query")).GetEnumerator();
          var dataSourceList= await  _dataSourceServcie.AllFromCache();

            while (apiList.MoveNext())
            {
                var item = apiList.Current;
                try
                {
                    var datasource = dataSourceList.First(f => f.Id == item.SourceId);
                    var dbtype = DbSchema.GetDbType(datasource.DbType);
                    ISqlParser sqlparser = SqlParserFactory.CreateParser(dbtype);

                    Console.WriteLine("\n\n");
                    Console.WriteLine($"Db:\t\t\t\t{dbtype}");
                    Console.WriteLine($"DBConn:\t\t\t{datasource.DbSchema.GetConnectionString()}");
                    Console.WriteLine($"sql:\t\t\t{item.ExecuteConfig.sqlText}");
                    var tableNames = sqlparser.ExtractTableNames(item.ExecuteConfig.sqlText);
                    Console.WriteLine($"tableNames:\t\t{string.Join(',', tableNames)}");
                    var fields = sqlparser.ExtractFields(item.ExecuteConfig.sqlText);
                    var fieldsStr = string.Join(',', fields);
                    Console.WriteLine($"Fields:\t\t{fieldsStr}");
                    Console.WriteLine($"ExtractConditions:\t\t{string.Join(' ', sqlparser.ExtractConditions(item.ExecuteConfig.sqlText))}");
                    Console.WriteLine($"ExtractConditions:\t\t{sqlparser.BuildConditions(item.ReqParams)}");
                    Assert.Equal(item.ExecuteConfig.tableName, tableNames[0]);
                    Assert.Equal(string.Join(' ', sqlparser.ExtractConditions(item.ExecuteConfig.sqlText)), sqlparser.BuildConditions(item.ReqParams));
                    Console.WriteLine("\n\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }



        /// <summary>
        /// 测试from的sql过滤条件解析
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestParser2()
        {
            ISqlParser sqlparser = SqlParserFactory.CreateParser(ITPortal.Core.DataSource.DbType.MYSQL);

            Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "pageSize", 20 },
            { "pageNum", 1 },
            { "building_name", "1891683815633395714" },
            { "create_time$", "2025-02-18%2010:59:02" },
            { "send_platform_time", "Dev-ST001" },
            { "platform_accept_time", "Dev-ST001" },
            { "invt_send_time", "Dev-ST001" },
            { "invt_declare_time", "Dev-ST001" },
            { "invt_preapproval_pass_time", "Dev-ST001" },
            { "bill_send_time", "Dev-ST001" },
            { "bill_release_time", "Dev-ST001" },
            { "bill_declare_time", "CNHUANB4246aa20250218105902095" },
            { "sync_update_time_start", "2025-02-18%2010:59:02.0" },
            { "sync_update_time_end", "undefined" }
        };
            //${AND LOWER(building_name) = LOWER(:building_name)}
            var sql =   sqlparser.ReplaceConditions(@"${AND LOWER(building_name) = LOWER(:building_name)} ${AND create_time$ = :create_time$} ${AND send_platform_time IS NULL} ${AND platform_accept_time  IS NOT NULL} 
${AND invt_send_time > :invt_send_time} ${AND invt_declare_time >= :invt_declare_time} ${AND invt_preapproval_pass_time < :invt_preapproval_pass_time} 
${AND bill_send_time <= :bill_send_time} ${AND bill_declare_time <> :bill_declare_time} ${AND bill_release_time IN (:bill_release_time)} 
${AND sync_update_time BETWEEN :sync_update_time_start AND :sync_update_time_end}",
                data);
            Console.WriteLine(sql);
        }


        /// <summary>
        /// 测试from的sql过滤条件解析
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestFromSqlFilterParser()
        {
            var list = await _apiservcie.AllFromCache();
            var apiList = list.Where(f => f.ApiUrl.EndsWith("/query")).GetEnumerator();
            var dataSourceList = await _dataSourceServcie.AllFromCache();
            System.Random random = new Random((int)DateTimeOffset.Now.Ticks);
            while (apiList.MoveNext())
            {
                var item = apiList.Current;
                try
                {
                    var datasource = dataSourceList.First(f => f.Id == item.SourceId);
                    var dbtype = DbSchema.GetDbType(datasource.DbType);
                    ISqlParser sqlparser = SqlParserFactory.CreateParser(dbtype);

                    #region data
                    Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "pageSize", 20 },
            { "pageNum", 1 },
            //{ "id", "1891683815633395714" },
            //{ "station_name", "Dev-ST001" },
            //{ "create_time", "2025-02-18%2010:59:02" },
            //{ "terminal_id", "Dev-ST001-CNHUANB4246" },
            //{ "transaction_no", "CNHUANB4246aa20250218105902095" },
            //{ "request_time_start", "2025-02-18%2010:59:02.0" },
            //{ "request_time_end", "undefined" },
            //{ "serial_number", "20250103A017" },
            //{ "computer_name", "CNHUANB4246" },
            //{ "process", null },
            //{ "make_result", null },
            //{ "start_time_str", "20250218105902095" },
            //{ "end_time_str", "20250218105902095" },
            //{ "start_time_start", "2025-02-18%2010:59:02.0" },
            //{ "start_time_end", "undefined" },
            //{ "end_time_start", "2025-02-18%2010:59:02.0" },
            //{ "end_time_end", "undefined" },
            //{ "equipment_serial_number", "123456" },
            //{ "dispensing_start_time_str", "20250218105902095" },
            //{ "dispensing_end_time_str", "20250218105902095" },
            //{ "dispensing_start_time_start", "2025-02-18%2010:59:02.0" },
            //{ "dispensing_start_time_end", "undefined" },
            //{ "dispensing_end_time_start", "2025-02-18%2010:59:02.0" },
            //{ "dispensing_end_time_end", "undefined" },
            //{ "dispensing_width", "5.70-4.19-4.59" },
            //{ "dispensing_height", "2.85-2.62-2.70" },
            //{ "dispensing_location", "" },
            //{ "dispensing_area", "" },
            //{ "glue_pn", 12 },
            //{ "glue_batch", "d233" },
            //{ "glue_date_code", "" },
            //{ "glue_opening_time_str", "20250113121300000" },
            //{ "glue_opening_time_start", "2025-01-13%2012:13:00.0" },
            //{ "glue_opening_time_end", "undefined" },
            //{ "glue_standing_time_str", "20250214141016004" },
            //{ "glue_standing_time_start", "2025-02-14%2014:10:16.0" },
            //{ "glue_standing_time_end", "undefined" },
            //{ "glue_viscosity", "" },
            //{ "glue_thixotropy", "" },
            //{ "glue_hardness", "" },
            //{ "glue_tg", null },
            //{ "glue_tensile_strength", "" },
            //{ "glue_shear_strength", "" },
            //{ "glue_young_modulus", "" },
            //{ "glue_poisson_ratio", "" },
            //{ "glue_density", "" },
            //{ "glue_curing_time_str", null },
            //{ "glue_curing_time_start", null },
            //{ "glue_curing_time_end", "undefined" },
            //{ "glue_curing_method", "" },
            //{ "glue_curing_temperature", "" },
            //{ "glue_uv_wave_length", "" },
            //{ "glue_uv_power", "" },
            //{ "glue_type", "" },
            //{ "glue_curing_humidity", "" },
            //{ "glue_expiration_date", "" },
            //{ "glue_cte", null },
            //{ "glue_shrinkage_rate", "" },
            //{ "glue_thermal_conductivity", "" },
            //{ "glue_amount", 4.54 },
            //{ "glue_speed", 3.2 },
            //{ "glue_pressure_a", 273 },
            //{ "glue_pressure_b", 267 },
            //{ "sn", "20250103A017" },
            //{ "work_cell", null },
            //{ "device_name", "FXK2-2" },
            //{ "pull_number", "BM08" },
            //{ "product_inspection_result", "Pass" },
            //{ "visual_positioning_compensation_value", "" },
            //{ "dispensing_start_position", "103.10-205.80--13.00" },
            //{ "dispensing_end_position", "169.13-284.16-0.50" },
            //{ "a_b_glue_ratio", "" },
            //{ "glue_weight_specification_spc", "" },
            //{ "last_measured_glue_weight", "" },
            //{ "compensation_for_needle_alignment", "" },
            //{ "coordinates_of_the_positioning_feature_point1", "275.0182-245.7302" },
            //{ "coordinates_of_positioning_feature2", "66.7325-158.7491" },
            //{ "visual_test_height_specification", "" },
            //{ "visual_test_height", "" },
            //{ "visual_test_results", "" },
            //{ "product_ok_quantity", "" },
            //{ "number_of_ng_products", "" },
            //{ "production_cycle", "" },
            //{ "create_by", "SYSTEM" },
            //{ "update_time_start", "2025-02-18%2010:59:02.0" },
            //{ "update_time_end", "undefined" },
            //{ "update_by", "SYSTEM" },
            //{ "is_deleted", 0 },
            //{ "equipment_id", null },
            //{ "machine_sn", null },
            //{ "jabil_sn", null },
            //{ "sync_update_time_start", "2025-03-03%2015:53:31.0" },
            //{ "sync_update_time_end", "undefined" }
        };

                    #endregion  data
                    var i = 1;
                    item.ReqParams.ForEach(x =>
                    {
                        if (random.Next(i++) % 2 == 0)
                            data.Add(x.paramName, x.defaultValue);
                    });

                    Console.WriteLine("\n\n");
                    var buildConditions = string.Join("", sqlparser.BuildConditions(item.ReqParams, data));
                    //var sqlText = Regex.Replace(item.ExecuteConfig.SqlText, @"\s+", " ");
                    var replaceConditions = sqlparser.ReplaceConditions(item.ExecuteConfig.sqlText, data);
                    var index = replaceConditions.IndexOf(buildConditions);
                    Console.WriteLine($"sqlText:\t\t\t{item.ExecuteConfig.sqlText}");
                    Console.WriteLine($"BuildConditions:\t\t{buildConditions}");
                    Console.WriteLine($"ReplaceConditions:\t\t{replaceConditions}");
                    Console.WriteLine($"index:\t\t{index}");
                    Assert.True(index > 0);
                    Console.WriteLine("\n\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        ///// <summary>
        ///// 在运行该类的任何测试之前，该方法只会为测试类调用一次。
        ///// </summary>
        ///// <param name="context"></param>
        //[ClassInitialize]
        //public static void ClassInit(TestContext context)
        //{
        //    _context = context;
        //    // 注册服务
        //    //services.AddSqlsugarSetup(App.Configuration);
        //    services.AddRedisCache();
        //    //services.AddSingleton<RedisCacheService>();
        //    services.AddSingleton<SwaggerDynamicConfigService>();
        //    services.AddTransient<IDataSourceFactory, CacheDbQueryFactory>();
        //    services.AddTransient<IDataApiLogService, DataApiLogService>();
        //    services.AddTransient<IDataApiService, DataApiService>();
        //    services.AddTransient<IDataSourceService, DataSourceService>();
        //    services.AddTransient<IApiServices, ApiSqlServiceHandler>();
        //    var configConnection = new ConnectionConfig()
        //    {
        //        DbType = SqlSugar.DbType.MySql,
        //        ConnectionString = "server=10.114.17.125;Database=dataxaccess;Uid=jabil;Pwd=jabil",
        //        IsAutoCloseConnection = true,
        //        MoreSettings = new ConnMoreSettings()
        //        {
        //            IsWithNoLockQuery = true,
        //        }
        //    };
        //    SqlSugarScope? sqlSugar = new SqlSugarScope(configConnection,
        //        db =>
        //        {
        //            //单例参数配置，所有上下文生效
        //            Console.WriteLine(db.CurrentConnectionConfig.ConnectionString);

        //            db.Aop.OnLogExecuting = (sql, pars) =>
        //            {
        //                Console.WriteLine(sql);//输出sql
        //            };
        //        });

        //    services.AddSingleton<ISqlSugarClient>(sqlSugar);

        //    services.AddSwaggerGen(async c =>
        //    {
        //        c.SwaggerDoc("DataAssetManager", new OpenApiInfo
        //        {
        //            Title = "数据资产API",
        //            Description = "数据资产API服务中心，包含数据资产对外提供的所有动态api",
        //            Contact = new OpenApiContact { Name = "数据资产团队", Email = "yang_li9954@jabil.com" },
        //            Version = "v1"
        //        });
        //    });


        //    // 构建服务提供程序
        //    var serviceProvider = services.BuildServiceProvider();

        //    // 解析服务
        //    _apiLogService = serviceProvider.GetRequiredService<IDataApiLogService>();
        //    _apiservcie = serviceProvider.GetRequiredService<IDataApiService>();
        //    _dataSourceServcie = serviceProvider.GetRequiredService<IDataSourceService>();
        //    // 
        //    // This method is called once for the test class, before any tests of the class are run.
        //}

        ///// <summary>
        ///// 在运行该类的所有测试之后，将为测试类调用此方法一次。
        ///// </summary>
        //[ClassCleanup]
        //public static void ClassCleanup()
        //{
        //    // This method is called once for the test class, after all tests of the class are run.
        //}

        ///// <summary>
        ///// 在每个测试方法之前都会调用此方法。
        ///// </summary>
        //[TestInitialize]
        //public void TestInit()
        //{
        //    // This method is called before each test method.
        //}

        ///// <summary>
        ///// 每次测试方法之后都会调用此方法。
        ///// </summary>
        //[TestCleanup]
        //public void TestCleanup()
        //{
        //    // This method is called after each test method.
        //}
    }
}
