using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch.Security;
using Elastic.Transport;

using Furion;
using Furion.EventBus;
using Furion.UnifyResult;

using Grpc.Core;

using ITPortal.Core;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.Services;
using ITPortal.Core.WebApi;
using ITPortal.Extension.System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Text;

namespace DataAssetManager.DataApiServer.Web.Core
{
    public class ActionResultFilter : IActionFilter
    {
        private readonly IUnifyResultProvider _resultProvider;
        private readonly ILogger<ActionResultFilter> _logger;
        private readonly IEventPublisher _eventPublisher;
        private readonly TrackLogProxyService _trackLog;
        private readonly Stopwatch stopwatch;
        public ActionResultFilter(IUnifyResultProvider resultProvider, TrackLogProxyService trackLog, ILogger<ActionResultFilter> logger, IEventPublisher eventPublisher)
        {
            _resultProvider = resultProvider;
            _logger = logger;
            _trackLog = trackLog; 
            _eventPublisher = eventPublisher;
            stopwatch = Stopwatch.StartNew();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            stopwatch.Stop();
            var exceptionMetadata = UnifyContext.GetExceptionMetadata(context);
            // 操作方法执行后的逻辑
            // 例如，统一处理返回结果
            if (context.Exception != null)
            {
                _logger.LogError(context.Exception, context.Exception.Message);
                context.ExceptionHandled = true;
                context.Result = new JsonResult(RESTfulResult(StatusCodes.Status500InternalServerError, false, errors: $"[{exceptionMetadata.ErrorCode}]{exceptionMetadata.Errors}"));
                var _ = LogFullTrackingInfo(context.HttpContext, stopwatch.ElapsedMilliseconds, false, exceptionMetadata.Errors?.ToString() ?? "", 503);

            }
            else if (context.Result is ObjectResult)
            {
                var result = (context.Result as ObjectResult)?.Value;
                if (result is not ITPortal.Core.Services.IResult)
                    context.Result = _resultProvider.OnSucceeded(context, (context.Result as ObjectResult)?.Value);
                var _ = LogFullTrackingInfo(context.HttpContext, stopwatch.ElapsedMilliseconds, true, context.Result.ToJSON());
            }
            else if (context.HttpContext.Response.StatusCode == 401)
            {
                context.Result = _resultProvider.OnAuthorizeException(new DefaultHttpContext(), exceptionMetadata);

                var _ = LogFullTrackingInfo(context.HttpContext, stopwatch.ElapsedMilliseconds, false, exceptionMetadata.Errors?.ToString() ?? "", 401);

            }
            else if (context.Result is JsonResult)
            {
                var result = context.Result as JsonResult;
                if (result.StatusCode == 200)
                    context.Result = _resultProvider.OnSucceeded(context, true);
                else
                    context.Result = new JsonResult(RESTfulResult(StatusCodes.Status400BadRequest, false, result.Value)
                , UnifyContext.GetSerializerSettings(context));

                var _ = LogFullTrackingInfo(context.HttpContext, stopwatch.ElapsedMilliseconds, result.StatusCode == 200, exceptionMetadata.Errors?.ToString() ?? context.Result?.ToJSON() ?? "", result.StatusCode.Value);
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = _resultProvider.OnSucceeded(context, true);

                var _ = LogFullTrackingInfo(context.HttpContext, stopwatch.ElapsedMilliseconds, true, context.Result.ToJSON());
            }
            else if (context.Result is IActionResult)
            {
                var _ = LogFullTrackingInfo(context.HttpContext, stopwatch.ElapsedMilliseconds, true, "");
            }
            else
            {
                var _ = LogFullTrackingInfo(context.HttpContext, stopwatch.ElapsedMilliseconds, true, context.Result?.ToJSON() ?? "");

            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            stopwatch.Start();
        }


        /// <summary>
        /// 返回 RESTful 风格结果集
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="succeeded"></param>
        /// <param name="data"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static Result<object> RESTfulResult(int statusCode, bool succeeded = default, object data = default, object errors = default)
        {
            return new Result<object>
            {
                Code = statusCode,
                Success = succeeded,
                Data = data,
                Msg = errors ?? (succeeded ? "success" : "failure"),
                Extras = UnifyContext.Take(),
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };
        }


        private async Task LogFullTrackingInfo(HttpContext context, long elapsedMs, bool? success = null, string error = null, int statusCode = 200)
        {
            if (context.Request.Path.Equals("/api/TrackLog", StringComparison.CurrentCultureIgnoreCase)) return;
            var actionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
            //var actionName = actionDescriptor != null
            //    ? $"{actionDescriptor.ControllerName}.{actionDescriptor.ActionName}"
            //    : "UnknownAction";
            //if (actionDescriptor != null && !actionDescriptor.DisplayName.StartsWith("DataAssetManager.DataApiServer.Application.DataApi"))
            //{
            var actionName = actionDescriptor != null ? actionDescriptor.DisplayName.Substring(0, actionDescriptor.DisplayName.IndexOf('(') - 1) : "UnknownAction"; ;
            //}
            var clientIp = IPAddressUtils.GetClientIp(context.Request);
            //var paramsData = await GetAllParams(context);

            var userid = context.GetCurrUserInfo()?.UserId;
            if (userid.IsNullOrWhiteSpace())
            {
                userid = App.HttpContext.Request.Headers["x-token"];
            }
            if (userid.IsNullOrWhiteSpace())
            {
                userid = "Anonymous";
            }
            ApiTrackLogInfo trackingInfo = new ApiTrackLogInfo()
            {
                Id = Guid.NewGuid(),
                Path = context.Request.Path,
                Method = context.Request.Method,
                StatusCode = statusCode,
                UserId = userid,
                ApiAction = actionName,
                ClientIp = clientIp,
                //RequestParameters = Newtonsoft.Json.JsonConvert.SerializeObject(paramsData),
                Msg = error,
                Success = success,
                ElapsedMilliseconds = elapsedMs
            };
            if (context.Items.TryGetValue("ApiTrackLog", out object logInfo))
            {
                var log = logInfo as ApiTrackLogInfo;
                trackingInfo.CallerDate = log.CallerDate;
                trackingInfo.ElapsedMilliseconds = (long)(DateTimeOffset.Now - log.CallerDate).TotalMilliseconds;
                trackingInfo.RequestParameters = log.RequestParameters;
            }
            if (actionDescriptor != null && actionDescriptor.DisplayName.StartsWith("DataAssetManager.DataApiServer.Application.DataApi"))
            {
                var _ = _eventPublisher.PublishAsync(new ElasticEventSource(DataAssetManagerConst.TrackLogRecordEvent, typeof(ApiTrackLogInfo), trackingInfo));
            }
            else
            {
                await _trackLog.TrackLog(trackingInfo);
            }
            //await Task.Run(async () =>
            //{
            //    await _trackLog.TrackLog(trackingInfo);
            //});
            //await _eventPublisher.PublishAsync(new ElasticEventSource(DataAssetManagerConst.TrackLogRecordEvent, typeof(ApiTrackLogInfo), trackingInfo));
        }


        public async Task<Dictionary<string, object>> GetAllParams(HttpContext context)
        {
            var request = context.Request;
            var paramsData = await ReadRequestBody(context);
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
            if (requestBody.StartsWith('{') || requestBody.StartsWith('['))
                return Furion.JsonSerialization.JSON.Deserialize<T>(requestBody);
            return default;
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
    }
}