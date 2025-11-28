using DataAssetManager.DataTableServer.Application;

using Furion;
using Furion.JsonSerialization;

using StackExchange.Profiling.Internal;

using System.Diagnostics;
using System.Net;
using System.Text;

using Xunit.Abstractions;

namespace DataAssetManager.xUnitTest
{

    public class DynamicRouteTest
    {
        // 创建服务集合
        //static ServiceCollection services = new ServiceCollection();
        private static IDataApiLogService _apiLogService;
        //private static IDataApiService _apiservcie;
        //private static IDataSourceService _dataSourceServcie;
        private readonly ITestOutputHelper Output;

        /// <summary>
        /// 这里不能通过构造函数注入，而是采用 App.GetService<> 方式
        /// </summary>
        public DynamicRouteTest(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
            _apiLogService = App.GetService<IDataApiLogService>();
            //_apiservcie = App.GetService<IDataApiService>();
            //_dataSourceServcie = App.GetService<IDataSourceService>();
        }



        /// <summary>
        /// 测试sqlquery查询接口
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestSqlQuery2()
        {
            var sw = new Stopwatch();
            sw.Start();
            // 发送请求
            var api = "http://cnhuam0itds01/gateway/dataasset/services/v1.0.0/trace/SCC/tp_production_line_personnel_working_hours/sqlQuery";
            var data = new
            {
                sqlText = "SELECT * FROM tp_production_line_personnel_working_hours WHERE 1=1 and UPPER(workcell) in ('VALEO') and ( login_time >= DATE_SUB(CURDATE(), INTERVAL 3 MONTH) AND login_time < CURDATE() + INTERVAL 1 DAY)",
                pageSize = 1000,
                pageNum = 1,
                total = 0
            };
            var response = await SendRequest("POST", api, data, "8f5ce529e77476008759ba6e2ede3e14");

            Output.WriteLine($"API: {api}");
            Output.WriteLine($"Post Data: {data}");
            Output.WriteLine($"ReadAsString: {await response.Content.ReadAsStringAsync()}");
        }



        /// <summary>
        /// 测试sqlquery查询接口
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestSqlQuery()
        {
            var list = _apiLogService.AsQueryable().Where(f=>f.CallerUrl.EndsWith("/sqlQuery",StringComparison.CurrentCultureIgnoreCase)
                                        && !f.CallerUrl.Contains("tp_employee_base_info") && !f.CallerUrl.Contains("HR_Attendance"))
                                        .OrderByDescending(f => f.CallerDate)
                                        .Take(10).ToList();

            Output.WriteLine("\r\n\n");
            // 模拟 1k QPS 测试
            int totalRequests = list.Count; // 总请求数
            var semaphore = new SemaphoreSlim(list.Count);
            var tasks = new List<Task>();
            var startTime = DateTimeOffset.Now;
            var i = 1;
            foreach (var item in list)
            {
                try
                {
                    if (item.CallerParam.sqlText.IsNullOrWhiteSpace()) continue;
                    await semaphore.WaitAsync();
                    tasks.Add(Task.Run(async () =>
                    {
                        StringBuilder logMsg = new StringBuilder();
                        try
                        {
                            var sw = new Stopwatch();
                            sw.Start();
                            // 发送请求
                            var api = $"https://localhost:5011{item.CallerUrl}";
                            var response = await SendRequest("POST", api, item.CallerParam,"");

                            // 处理响应
                            logMsg.AppendLine($"[{i}] API: [{item.ApiId}]{item.ApiName}");
                            logMsg.AppendLine($"[{i}] API: {api}");
                            logMsg.AppendLine("");
                            logMsg.AppendLine($"[{i}] CallerParams: {item.CallerParams}");
                            logMsg.AppendLine("");
                            logMsg.AppendLine($"[{i}] CallerParam: {JSON.Serialize(item.CallerParam)}");
                            logMsg.AppendLine("");
                            logMsg.AppendLine($"[{i}] sql: {item.CallerParam.sqlText}");
                            logMsg.AppendLine("");
                            logMsg.AppendLine($"[{i}] completed: {response.StatusCode}");
                            logMsg.AppendLine($"[{i}] data: {await response.Content.ReadAsStringAsync()}");
                            sw.Stop();

                            logMsg.AppendLine($"[{i}] time: {sw.ElapsedMilliseconds}ms");
                            logMsg.AppendLine("\r\n\n");
                            i++;
                        }
                        catch (Exception ex)
                        {
                            logMsg.AppendLine($"Request {i} failed: {ex.Message}");
                        }
                        finally
                        {
                            semaphore.Release();
                            Output.WriteLine(logMsg.ToString());
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"param:\t\t\t{item.CallerParams}");
                    Output.WriteLine(ex.StackTrace);
                }
            }
            // 等待所有任务完成
            await Task.WhenAll(tasks);

            var endTime = DateTimeOffset.Now;
            var elapsedSeconds = (endTime - startTime).TotalSeconds;
            Output.WriteLine($"Test completed in {elapsedSeconds} seconds.");
            Output.WriteLine($"Actual QPS: {totalRequests / elapsedSeconds}");
        }

        /// <summary>
        /// 表单接口请求测试
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestFromQuery()
        {
            var list = _apiLogService.AsQueryable().Where(f => f.CallerUrl.EndsWith("/query", StringComparison.CurrentCultureIgnoreCase) 
                                        && !f.CallerUrl.Contains("tp_employee_base_info")&& !f.CallerUrl.Contains("HR_Attendance")
                                        && !f.CallerUrl.Contains("Trace_DIIAutoProjection"))
                                        .OrderByDescending(f => f.CallerDate)
                                        .Take(5).ToList();

            Output.WriteLine("\r\n\n");
            // 模拟 1k QPS 测试
            int totalRequests = list.Count; // 总请求数
            var semaphore = new SemaphoreSlim(list.Count);
            var tasks = new List<Task>();
            var startTime = DateTimeOffset.Now;
            var index = 1;
            foreach (var item in list)
            {
                try
                {
                    await semaphore.WaitAsync();
                    tasks.Add(Task.Run(async () =>
                    {
                        StringBuilder logMsg = new StringBuilder();
                        var i = index++;
                        try
                        {
                            if (item.CallerParamDict.Count != 2 || item.CallerUrl.Contains("tp_employee_base_info")) return;
                            var sw = new Stopwatch();
                            sw.Start();
                            var api = $"https://localhost:5011{item.CallerUrl}";

                            //Output.WriteLine($"[{i}] API:              [{item.ApiId}]{item.ApiName}");
                            //Output.WriteLine($"[{i}] API:              {api}");
                            //Output.WriteLine($"[{i}] CallerParams:     {item.CallerParams}");
                            //Output.WriteLine($"[{i}] CallerParamDict:  {JSON.Serialize(item.CallerParamDict)}");

                            // 发送请求
                            var response = await SendRequest("POST", api, item.CallerParamDict);
                            sw.Stop();
                            // 处理响应
                            logMsg.AppendLine($"[{i}] time:             {sw.ElapsedMilliseconds}ms");
                            logMsg.AppendLine($"[{i}] API:              [{item.ApiId}]{item.ApiName}");
                            logMsg.AppendLine($"[{i}] API:              {api}");
                            logMsg.AppendLine($"[{i}] CallerParams:     {item.CallerParams}");
                            logMsg.AppendLine($"[{i}] CallerParamDict:  {JSON.Serialize(item.CallerParamDict)}");
                            logMsg.AppendLine($"[{i}] completed:        {response.StatusCode}");
                            logMsg.AppendLine($"[{i}] data:             {await response.Content.ReadAsStringAsync()}");

                            logMsg.AppendLine("");
                            logMsg.AppendLine("");
                            logMsg.AppendLine("");
                            logMsg.AppendLine("");
                        }
                        catch (Exception ex)
                        {
                            logMsg.AppendLine($"Request {i} failed: {ex.Message}");
                        }
                        finally
                        {
                            semaphore.Release();
                            Output.WriteLine(logMsg.ToString());
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"param:\t\t\t{item.CallerParams}");
                    Output.WriteLine(ex.StackTrace);
                }
            }
            // 等待所有任务完成
            await Task.WhenAll(tasks);

            var endTime = DateTimeOffset.Now;
            var elapsedSeconds = (endTime - startTime).TotalSeconds;
            Output.WriteLine($"Test completed in {elapsedSeconds} seconds.");
            Output.WriteLine($"Actual QPS: {totalRequests / elapsedSeconds}");
        }

        public async Task RunQPSTest(string method, string url, string sqlText, int pageNum, int pageSize, int totalRequests, int duration)
        {
            var semaphore = new SemaphoreSlim(totalRequests);
            var tasks = new List<Task>();
            var startTime = DateTimeOffset.Now;

            // 启动请求任务
            for (int i = 0; i < totalRequests; i++)
            {
                await semaphore.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    StringBuilder logMsg = new StringBuilder();
                    try
                    {
                        // 构建请求参数
                        var requestParams = new
                        {
                            sqlText,
                            pageNum,
                            pageSize
                        };

                        // 发送请求
                        var response = await SendRequest(method, url, requestParams);
                        // 处理响应
                        logMsg.AppendLine($"Request {i + 1} completed: {response.StatusCode}");
                    }
                    catch (Exception ex)
                    {
                        logMsg.AppendLine($"Request {i + 1} failed: {ex.Message}");
                    }
                    finally
                    {
                        semaphore.Release();
                        Output.WriteLine(logMsg.ToString());
                    }
                }));
            }

            // 等待所有任务完成
            await Task.WhenAll(tasks);

            var endTime = DateTimeOffset.Now;
            var elapsedSeconds = (endTime - startTime).TotalSeconds;
            Output.WriteLine($"Test completed in {elapsedSeconds} seconds.");
            Output.WriteLine($"Actual QPS: {totalRequests / elapsedSeconds}");
        }


        public async Task<HttpResponseMessage> SendRequest(string method, string url, object requestParams, string xtoken="")
        {
            var httpMethod = new HttpMethod(method);
            var request = new HttpRequestMessage(httpMethod, url);

            if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
            {
                request.Content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestParams),
                    System.Text.Encoding.UTF8,
                    "application/json");
            }
            else if (httpMethod == HttpMethod.Get)
            {
                if (requestParams != null)
                {
                    var queryString = GetQueryString(requestParams);
                    if (!string.IsNullOrEmpty(queryString))
                    {
                        url += (url.Contains("?") ? "&" : "?") + queryString;
                        request.RequestUri = new Uri(url);
                    }
                }
            }


            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("x-token", xtoken);
            return await httpClient.SendAsync(request);
        }

        private string GetQueryString(object obj)
        {
            var properties = new List<KeyValuePair<string, string>>();

            if (obj is IDictionary<string, object> dictionary)
            {
                foreach (var keyValue in dictionary)
                {
                    if (keyValue.Value != null)
                    {
                        properties.Add(new KeyValuePair<string, string>(keyValue.Key, keyValue.Value.ToString()));
                    }
                }
            }
            else
            {
                var type = obj.GetType();
                var props = type.GetProperties();
                foreach (var prop in props)
                {
                    var value = prop.GetValue(obj);
                    if (value != null)
                    {
                        properties.Add(new KeyValuePair<string, string>(prop.Name, value.ToString()));
                    }
                }
            }

            return string.Join("&", properties.Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}"));
        }


        ///// <summary>
        ///// 在运行该类的任何测试之前，该方法只会为测试类调用一次。
        ///// </summary>
        ///// <param name="context"></param>
        //[ClassInitialize]
        //public static void ClassInit(TestContext context)
        //{
        //    _context = context;

        //    var configuration = new ConfigurationBuilder()
        //       .SetBasePath(Directory.GetCurrentDirectory())
        //       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //       .Build();
        //    // 注册服务
        //    //services.AddSqlsugarSetup(App.Configuration);
        //    services.AddLog();
        //    services.AddRedisCache(option => {
        //        var data = configuration.GetValue(typeof(RedisCacheOptions), "Redis") as RedisCacheOptions; 
          
        //        option.Configuration = configuration["Redis:Configuration"];
        //    });
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
        //            Output.WriteLine(db.CurrentConnectionConfig.ConnectionString);

        //            db.Aop.OnLogExecuting = (sql, pars) =>
        //            {
        //                Output.WriteLine(sql);//输出sql
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

        //    services.AddJsonOptions(options =>
        //    {
        //        //定义json编码格式，处理中文乱码
        //        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        //    });


        //    // 构建服务提供程序
        //    var serviceProvider = services.BuildServiceProvider();
        //    var scope = serviceProvider.CreateScope();

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
