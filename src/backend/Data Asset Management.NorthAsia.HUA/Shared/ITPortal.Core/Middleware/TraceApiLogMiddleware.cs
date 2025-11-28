using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch.Security;

using Furion.EventBus;
using Furion.UnifyResult;

using Grpc.Core;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.Services;
using ITPortal.Core.WebApi;
using ITPortal.Extension.System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

using SqlSugar;

using System.Diagnostics;
using System.Text;

namespace DataAssetManager.DataApiServer.Web.Core
{
    public class TraceApiLogMiddleware
    {
        private readonly RequestDelegate _next;
        protected readonly IEventPublisher EventPublisher;
        protected readonly ILogger<TraceApiLogMiddleware> _logger;

        public TraceApiLogMiddleware(RequestDelegate next, IEventPublisher eventPublisher, ILogger<TraceApiLogMiddleware> logger)
        {
            _next = next;
            EventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Items.Add("ApiTrackLog", new ApiTrackLogInfo()
            {
                Id = Guid.NewGuid(),
                Path = context.Request.Path,
                Method = context.Request.Method,
                RequestParameters = (GetAllParams(context).Result).ToJSON()
            });
            await _next(context);
        }

        public async Task<Dictionary<string, object>> GetAllParams(HttpContext context)
        {
            var request = context.Request;
            if (request.Path.HasValue && request.Path.Value.StartsWith("/services/")) return new Dictionary<string, object>();
            var paramsData = await ReadRequestBody(context) ?? new Dictionary<string, object>();
            paramsData = ToDynamic(request.Query, paramsData);
            if (request.HasFormContentType) paramsData = ToDynamic(request.Form, paramsData);
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
            try
            {
                var data = Furion.JsonSerialization.JSON.Deserialize<T>(requestBody);
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return default;
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
    }
}