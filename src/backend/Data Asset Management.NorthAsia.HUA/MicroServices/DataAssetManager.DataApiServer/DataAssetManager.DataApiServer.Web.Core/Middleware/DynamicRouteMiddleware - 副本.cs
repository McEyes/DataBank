
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAssetManager.Core.Extensions;
using Azure.Core;
using System.IO;
using DataAssetManager.Core.Services;
using DataAssetManager.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using DataAssetManager.DataApiServer.Core;
using DataAssetManager.DataApiServer.Application;
using DataAssetManager.DataApiServer.Application.Services;


namespace DataAssetManager.DataApiServer.Web.Core
{
    public class DynamicRouteMiddleware
    {
        private readonly RedisCacheService _cache;
        private readonly RequestDelegate _next;

        //public static readonly Dictionary<string, RouteInfo> _activeRoutes = new Dictionary<string, RouteInfo>();

        public DynamicRouteMiddleware(RequestDelegate next, RedisCacheService cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";
            var routInfo = _cache.HashGet<RouteInfo>(DataAssetManagerConst.RouteRedisKey, path);
            if (routInfo != null)
            {

            //    检查访问频率
               if (!_rateLimiter.IsAllowed(request.ApiUrl, request.Limit))
               {
                  context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                  await context.Response.WriteAsync("Too many requests.");
                  return;
               }

               // 检查参数
               if (!RequestValidator.Validate(context.Request, request.ReqJson))
               {
                  context.Response.StatusCode = StatusCodes.Status400BadRequest;
                  await context.Response.WriteAsync("Invalid request parameters.");
                  return;
               }

                var databaseService = context.RequestServices.GetRequiredService<ApiHandler>();
                var paramsBody = await ReadRequestBody(context);
                PageResult<object> result = null;// await databaseService.Execute(context.Request, apiInfo: routInfo, paramsBody);
                var sw = new Stopwatch();
                sw.Start();
                result = await databaseService.Execute(context.Request, apiInfo: routInfo, paramsBody);
                sw.Stop();
                await context.Response.WriteAsJsonAsync(result);
                Console.WriteLine($"收到{path}请求：{Newtonsoft.Json.JsonConvert.SerializeObject(routInfo)},耗时：{sw.ElapsedMilliseconds}毫秒");
                return;
            }
            await _next(context);
        }


        private async Task InvokeBackAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";
            var routInfo = _cache.HashGet<RouteInfo>(DataAssetManagerConst.RouteRedisKey, path);
            if (routInfo != null)
            {
                var databaseService = context.RequestServices.GetRequiredService<ApiHandler>();
                var paramsBody = await ReadRequestBody(context);
                PageResult<object> result = null;// await databaseService.Execute(context.Request, apiInfo: routInfo, paramsBody);
                var sw = new Stopwatch();
                sw.Start();
                result = await databaseService.Execute(context.Request, apiInfo: routInfo, paramsBody);
                sw.Stop();
                await context.Response.WriteAsJsonAsync(result);
                Console.WriteLine($"收到{path}请求：{Newtonsoft.Json.JsonConvert.SerializeObject(routInfo)},耗时：{sw.ElapsedMilliseconds}毫秒");
                return;
            }

            //if (_activeRoutes.TryGetValue(path, out RouteInfo? request))
            //{
            //    // 检查访问频率
            //    //if (!_rateLimiter.IsAllowed(request.ApiUrl, request.Limit))
            //    //{
            //    //    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            //    //    await context.Response.WriteAsync("Too many requests.");
            //    //    return;
            //    //}

            //    //// 检查参数
            //    //if (!RequestValidator.Validate(context.Request, request.ReqJson))
            //    //{
            //    //    context.Response.StatusCode = StatusCodes.Status400BadRequest;
            //    //    await context.Response.WriteAsync("Invalid request parameters.");
            //    //    return;
            //    //}

            //    //// 处理请求
            //    //await context.Response.WriteAsync($"This is a dynamically registered route: {request.ApiUrl}");

            //    //// 处理请求
            //    //var result = _databaseService.ExecuteSql(request);
            //    //await context.Response.WriteAsJsonAsync(result);
            //    Console.WriteLine($"收到{path}请求：" + Newtonsoft.Json.JsonConvert.SerializeObject(request));
            //    return;
            //}

            await _next(context);
        }

        //public static async void RegisterRoute(RouteInfo config)
        //{
        //    if (config.Status == "2")
        //    {
        //        //endpoints.MapGet(config.ApiUrl, async context =>
        //        //{
        //        //    var databaseService = context.RequestServices.GetRequiredService<DatabaseService>();
        //        //    var result = databaseService.ExecuteSql(config.ConnectionString, config.SqlQuery, config.Parameters, config.DbType);
        //        //    await context.Response.WriteAsJsonAsync(result);
        //        //});
        //        // 存储路由配置
        //        DynamicRouteConfigurations.Routes[config.ApiUrl] = config;

        //        _activeRoutes[config.ApiUrl] = config;
        //    }
        //    else
        //    {
        //        Console.WriteLine($"[{config.ApiUrl}]不是发布状态[{config.Status}]，跳过注册。");
        //    }
        //}

        //public static void UnRegisterRoute(string path)
        //{
        //    _activeRoutes.Remove(path);
        //    // 存储路由配置
        //    DynamicRouteConfigurations.Routes.Remove(path);
        //    Console.WriteLine($"[{path}]注册删除成功。");
        //}

        public async Task<Dictionary<string, object>> ReadRequestBody(HttpContext context)
        {
            return await ReadRequestBody<Dictionary<string, object>>(context);
        }

        public async Task<T> ReadRequestBody<T>(HttpContext context)
        {
            // 确保请求体流可重复读取
            context.Request.EnableBuffering();

            // 读取请求体
            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);
            // 输出请求体数据
            System.Console.WriteLine($"Request Body: {requestBody}");
            // 将流指针重置到开头，以便后续中间件或控制器可以再次读取请求体
            context.Request.Body.Position = 0;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(requestBody);
        }
    }
}