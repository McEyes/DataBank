using Furion;
using Furion.Authorization;
using Furion.DataEncryption;
using Furion.EventBus;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.WebApi;

using ITPortal.Extension.System;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

using System.Text;

namespace ITPortal.Core
{
    public class DefaultJwtHandler : AppAuthorizeHandler
    {
        private ILogger<DataAssetJwtHandler> _logger;
        private readonly TrackLogProxyService _trackLog;
        private readonly IEventPublisher _eventPublisher;
       public DefaultJwtHandler(ILogger<DataAssetJwtHandler> logger, TrackLogProxyService trackLog, IEventPublisher eventPublisher)
        {
            _logger = logger;
            _trackLog = trackLog;
            _eventPublisher = eventPublisher;
        }
        /// <summary>
        /// 重写 Handler 添加自动刷新收取逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public override async Task HandleAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
        {
            var sw = new Stopwatch();
            sw.Start();
            var cont = context.GetCurrentHttpContext();
            // 自动刷新 token
            var path = context.GetCurrentHttpContext().Request.Path.Value ?? "";
            path = path.ToLower();
            try
            {
                if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext()))
                {
                    await AuthorizeHandleAsync(context);
                }
                else
                {
                    // 授权失败
                    sw.Stop();
                    var _ = SendTracLogEvent(context.GetCurrentHttpContext(), (int)sw.ElapsedMilliseconds, false, $"{path}, HTTP status code 401.");
                    _logger.LogError($"验证权限失败：{httpContext.Request.Path}");
                    context.Fail();
                }
            }
            catch (Exception ex)
            {
                // redis 异常
                sw.Stop();
                var _ = SendTracLogEvent(context.GetCurrentHttpContext(), (int)sw.ElapsedMilliseconds, false, $"{path}, HTTP status code 401..\r\n{ex.Message},\r\n{ex.StackTrace}");
                _logger.LogError($"验证权限异常：{httpContext.Request.Path},\r\n{ex.Message},\r\n{ex.StackTrace}");
                context.Fail();
            }
        }

        //public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
        //{
        //    if (!httpContext.Items.ContainsKey(DataAssetManagerConst.HttpContext_UserInfo))
        //        httpContext.Items.Add(DataAssetManagerConst.HttpContext_UserInfo, new UserInfo(httpContext.User));
        //    // 这里写您的授权判断逻辑，授权通过返回 true，否则返回 false
        //    var result = CheckAuthorzie(httpContext);
        //    return await Task.FromResult(true);
        //}

        ///// <summary>
        ///// 检查权限
        ///// </summary>
        ///// <param name="httpContext"></param>
        ///// <returns></returns>
        //private static bool CheckAuthorzie(DefaultHttpContext httpContext)
        //{
        //    // 获取权限特性
        //    //var securityDefineAttribute = httpContext.GetMetadata<SecurityDefineAttribute>();
        //    //if (securityDefineAttribute == null) return true;

        //    return true;// "查询数据库返回是否有权限";
        //}

        //public async Task SendLogEvent(HttpContext context, RouteInfo routInfo, int times, string msg, int status = 0, int callerSize = 0)
        //{
        //    //记录api调用情况
        //    Dictionary<string, object> paramsData = await GetAllParams(context);
        //    var userid = context.GetCurrUserInfo()?.UserId;
        //    if (userid.IsNullOrWhiteSpace())
        //    {
        //        userid = App.HttpContext.Request.Headers["x-token"];
        //    }
        //    var apiLogDto = new
        //    {
        //        //Id = Guid.NewGuid().ToString().Replace("-", ""), 
        //        ApiId = routInfo.Id ?? "",
        //        ApiName = routInfo.ApiName ?? routInfo.ApiUrl,
        //        CallerUrl = routInfo.ApiServiceUrl,
        //        CallerParams = paramsData.ToJSON(),// JSON.Serialize(paramsData), // ToKeyValueStr(paramsData),
        //        CallerId = userid,
        //        CallerDate = DateTimeOffset.Now,
        //        CallerIp = IPAddressUtils.GetClientIp(context.Request),
        //        CallerSize = callerSize,
        //        OwnerDepart = routInfo.OwnerDepart,
        //        Status = status,
        //        Time = times,
        //        Msg = msg,
        //        TableId = routInfo.TableId,
        //    };
        //    var _ = _eventPublisher.PublishAsync(DataAssetManagerConst.LogRecordEvent, apiLogDto.ToJSONElement());
        //}

        public async Task SendTracLogEvent(HttpContext context, long elapsedMs, bool? success = null, string error = null)
        {
            //记录api调用情况
            if (context.Request.Path.Equals("/api/TrackLog", StringComparison.CurrentCultureIgnoreCase)) return;
            var actionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
            //var actionName = actionDescriptor != null
            //    ? $"{actionDescriptor.ControllerName}.{actionDescriptor.ActionName}"
            //    : "UnknownAction";   
            //var actionName = actionDescriptor != null
            //    ? $"{actionDescriptor.ControllerName}.{actionDescriptor.ActionName}"
            //    : "UnknownAction";
            //if (actionDescriptor != null && !actionDescriptor.DisplayName.StartsWith("DataAssetManager.DataApiServer.Application.DataApi"))
            //{
            var actionName = actionDescriptor != null ? actionDescriptor.DisplayName.Substring(0, actionDescriptor.DisplayName.IndexOf('(') - 1) : "UnknownAction"; ;
            //}
            var clientIp = IPAddressUtils.GetClientIp(context.Request);
            Dictionary<string, object> paramsData = await GetAllParams(context);
            var userid = context.GetCurrUserInfo()?.UserId;
            if (userid.IsNullOrWhiteSpace())
            {
                userid = App.HttpContext.Request.Headers["x-token"];
            }
            if (userid.IsNullOrWhiteSpace())
            {
                userid = "Anonymous";
            }
            var trackingInfo = new ApiTrackLogInfo()
            {
                Path = context.Request.Path,
                Method = context.Request.Method,
                StatusCode = 401,
                UserId = userid,
                ApiAction = actionName,
                ClientIp = clientIp,
                RequestParameters = Newtonsoft.Json.JsonConvert.SerializeObject(paramsData),
                Msg = error,
                Success = success,
                ElapsedMilliseconds = elapsedMs
            };
            if (actionDescriptor != null && actionDescriptor.DisplayName.StartsWith("DataAssetManager.DataApiServer.Application.DataApi"))
            {
                var _ = _eventPublisher.PublishAsync(new ElasticEventSource(DataAssetManagerConst.TrackLogRecordEvent, typeof(ApiTrackLogInfo), trackingInfo));
            }
            else
            {
                await _trackLog.TrackLog(trackingInfo);
            }
            //await _trackLog.TrackLog(trackingInfo);
            //var _ = _eventPublisher.PublishAsync(new ElasticEventSource(DataAssetManagerConst.TrackLogRecordEvent, typeof(ApiTrackLogInfo), trackingInfo));
        }

        public async Task<Dictionary<string, object>> GetAllParams(HttpContext context)
        {
            var request = context.Request;
            var paramsData = await ReadRequestBody(context);
            //ToDynamic(headers: request.Headers, paramsData);
            paramsData = ToDynamic(request.Query, paramsData);
            if (request.HasFormContentType) paramsData = ToDynamic(request.Form, paramsData);
            CheckPageParams(paramsData);
            return paramsData;
        }

        private async Task<Dictionary<string, object>> ReadRequestBody(HttpContext context)
        {
            return await ReadRequestBody<Dictionary<string, object>>(context);
        }

        private async Task<T> ReadRequestBody<T>(HttpContext context)
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
        private Dictionary<string, object> ToDynamic(IFormCollection from, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();
            if (from == null) return expando;

            foreach (var kvp in from)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }
        private Dictionary<string, object> ToDynamic(IQueryCollection query, Dictionary<string, object> expando = null)
        {
            if (expando == null) expando = new Dictionary<string, object>();
            if (query == null) return expando;

            foreach (var kvp in query)
            {
                expando[kvp.Key] = kvp.Value;
            }

            return expando;
        }

        private void CheckPageParams(Dictionary<string, object> expandoDict)
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
