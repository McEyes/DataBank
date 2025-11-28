
using DataAssetManager.DataApiServer.Application.DataApi;
using DataAssetManager.DataApiServer.Application.Services;
using DataAssetManager.DataTableServer.Application;

using Furion;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Extensions;
using ITPortal.Core.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace DataAssetManager.DataApiServer.Web.Core
{
    public class DynamicRouteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCacheService _cache;
        private readonly ILogger<DynamicRouteMiddleware> _logger;
        private readonly IDataApiLogService _apiLogService;
        /// <summary>
        /// 启用table配置的类型解析
        /// </summary>
        private readonly bool EnableTableColumnType;

        public DynamicRouteMiddleware(RequestDelegate next, IDataApiLogService apiLogService, IDistributedCacheService cache, ILogger<DynamicRouteMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
            _apiLogService = apiLogService;
            _cache.Set($"{DataAssetManagerConst.RedisKey}APITest", App.Configuration.GetValue<string>("APITest"));
            EnableTableColumnType = App.Configuration.GetValue<bool>("EnableTableColumnType");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";

            if (!path.StartsWith("/services/", StringComparison.CurrentCultureIgnoreCase))
            {
                await _next(context);
                return;
            }
            path = path.ToLower();
            var routInfo = _cache.HashGet<RouteInfo>(DataAssetManagerConst.RouteRedisKey, path);
            if (routInfo != null)
            {
                var sw = new Stopwatch();
                sw.Start();
                Dictionary<string, object> paramsData = null;
                IPageResult data = new PageResult();
                var status = 1;
                var msg = "";
                try
                {
                    context.Items["__NonUnify__"] = true;
                    paramsData = await GetAllParams(context);
                    var databaseService = ApiServiceFactory.CreateService(context, routInfo.ExecuteConfig.configType.GetEnum<ConfigType>());
                    if (EnableTableColumnType || (paramsData.TryGetValue("ConvertToTableClass", out object t) && "1".Equals(t)))
                    {
                        var resultData = new Result<PageResult>();
                        data = resultData.Data = await databaseService.ExecuteClass(context.Request, routInfo, paramsData);
                        await context.Response.WriteAsJsonAsync(resultData);
                    }
                    else if (paramsData.TryGetValue("to_excel", out object toExcel) && "1".Equals(toExcel))
                    {
                        var fileResult = await databaseService.ExecuteToExcel(context.Request, routInfo, paramsData);
                        var actionContext = new ActionContext(context, context.GetRouteData(), new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor());
                        await fileResult.ExecuteResultAsync(actionContext);
                    }
                    else
                    {
                        var resultData = new Result<PageResult<object>>();
                        data = resultData.Data = await databaseService.Execute(context.Request, routInfo, paramsData);
                        await context.Response.WriteAsJsonAsync(resultData);
                    }
                    sw.Stop();
                    //msg = $"data rows:{result.Data?.Data?.Count()}";
                }
                catch (Exception ex)
                {
                    _logger.LogError($"API[{routInfo.Id}]异常:{routInfo.ApiUrl}\r\n{ex.Message}\r\n{ex.StackTrace}");
                    status = 0;
                    msg = ex.Message;
                    await context.Response.WriteAsJsonAsync(new Result() { Code = StatusCodes.Status500InternalServerError, Success = false, Msg = ex.Message });
                }
                finally
                {
                    sw.Stop();
                    _logger.LogInformation($"收到{path}请求,耗时：{sw.ElapsedMilliseconds}毫秒");
                    var _ = _apiLogService.SendLogEvent(context, routInfo, (int)sw.ElapsedMilliseconds, msg, status, data?.Total ?? 0);
                }
                return;
            }
            else if (path.StartsWith("/services", StringComparison.CurrentCultureIgnoreCase))
            {
                _logger.LogError($"API[{path}]路由信息获取为空，从缓存中获取失败！");
            }
            await _next(context);
        }

        public async Task<Dictionary<string, object>> GetAllParams(HttpContext context)
        {
            var request = context.Request;
            var paramsData = await ReadRequestBody(context);
            paramsData = ToDynamic(request.Query, paramsData);
            if (request.HasFormContentType) paramsData = ToDynamic(request.Form, paramsData);
            CheckPageParams(paramsData);
            return paramsData;
        }


        public async Task<Dictionary<string, object>> ReadRequestBody(HttpContext context)
        {
            return await ReadRequestBody<Dictionary<string, object>>(context);
        }

        public async Task<T> ReadRequestBody<T>(HttpContext context)
        {
            // 确保请求体流可重复读取
            context.Request.EnableBuffering();
            if (!context.Request.ContentLength.HasValue || context.Request.ContentLength <= 0) return default;
            // 读取请求体
            //var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            //await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            //var requestBody = Encoding.UTF8.GetString(buffer);
            var requestBody = string.Empty;
            // 初始化内存流用于累积数据
            using (var ms = new MemoryStream())
            {
                // 定义每次读取的缓冲区（大小可根据实际场景调整，例如4096字节）
                byte[] buffer = new byte[4096];
                int bytesRead;

                // 循环读取，直到流结束（bytesRead为0）
                while ((bytesRead = await context.Request.Body.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    // 将读取到的字节写入内存流
                    await ms.WriteAsync(buffer, 0, bytesRead);
                }

                // 将内存流中的数据转换为字符串（使用UTF8编码）
                requestBody = Encoding.UTF8.GetString(ms.ToArray());
            }
            // 输出请求体数据
            //System.Console.WriteLine($"Request Body: {requestBody}");
            requestBody = requestBody.Trim('\0');
            // 将流指针重置到开头，以便后续中间件或控制器可以再次读取请求体
            context.Request.Body.Position = 0;
            return Furion.JsonSerialization.JSON.Deserialize<T>(requestBody);
        }

        public Dictionary<string, object> ToDynamic(Dictionary<string, object> dict, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();

            foreach (var kvp in dict)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }

        public Dictionary<string, object> ToDynamic(Dictionary<string, string> dict, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();

            foreach (var kvp in dict)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }

        public Dictionary<string, object> ToDynamic(IFormCollection from, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();
            if (from == null) return expando;

            foreach (var kvp in from)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }

        public Dictionary<string, object> ToDynamic(IHeaderDictionary headers, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();
            if (headers == null) return expando;

            foreach (var kvp in headers)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }

        public Dictionary<string, object> ToDynamic(IQueryCollection query, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();
            if (query == null) return expando;

            foreach (var kvp in query)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }

        public void CheckPageParams(Dictionary<string, object> expandoDict)
        {
            if (!expandoDict.ContainsKey("pageNum")) expandoDict.Add("pageNum", 1);
            if (!expandoDict.ContainsKey("pageSize"))
            {
                if (!expandoDict.ContainsKey("maxSize"))
                    expandoDict.Add("pageSize", value: 5000);
                else
                    expandoDict.Add("pageSize", expandoDict["maxSize"]);
            }
            if (!expandoDict.ContainsKey("total")) expandoDict.Add("total", 0);
        }


    }
}