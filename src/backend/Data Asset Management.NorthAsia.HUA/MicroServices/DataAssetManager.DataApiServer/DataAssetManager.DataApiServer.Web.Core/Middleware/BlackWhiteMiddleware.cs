using DataAssetManager.DataTableServer.Application;

using Furion.UnifyResult;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.WebApi;

using Microsoft.AspNetCore.Http;

using SqlSugar;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Web.Core
{
    public class BlackWhiteMiddleware
    {
        private readonly IDistributedCacheService _cache;
        private readonly RequestDelegate _next;
        private readonly IDataApiLogService _apiLogService;

        public BlackWhiteMiddleware(RequestDelegate next, IDistributedCacheService cache,IDataApiLogService dataApiLogService)
        {
            _next = next;
            _cache = cache;
            _apiLogService = dataApiLogService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            var path = context.Request.Path.Value ?? "";
            if (!path.StartsWith("/services/", StringComparison.CurrentCultureIgnoreCase))
            {
                await _next(context);
                return;
            }
            path = path.ToLower();
            var routInfo = _cache.HashGet<RouteInfo>(DataAssetManagerConst.RouteRedisKey, path);
            if (routInfo != null && routInfo.Deny != null)
            {
                var ip = IPAddressUtils.GetClientIp(context.Request);
                if (routInfo.Deny.Split(',').Any(f => f == ip))
                {
                    context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                    await context.Response.WriteAsJsonAsync(new RESTfulResult<string>() { Errors = $"API{routInfo.Id},The IP({ip}) is already in blacklist, not allowed to access!." });

                    //记录api调用情况
                    sw.Stop();
                    var _ = _apiLogService.SendLogEvent(context, routInfo, (int)sw.ElapsedMilliseconds, $"API{routInfo.Id},The IP({ip}) is already in blacklist, not allowed to access!.");
                    return;
                }
            }
            sw.Stop();
            await _next(context);
        }
    }
}