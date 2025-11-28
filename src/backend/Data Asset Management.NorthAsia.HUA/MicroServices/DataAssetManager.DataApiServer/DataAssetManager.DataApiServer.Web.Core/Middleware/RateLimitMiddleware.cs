
using DataAssetManager.DataApiServer.Application;
using DataAssetManager.DataTableServer.Application;

using Furion.UnifyResult;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;

using Microsoft.AspNetCore.Http;

using MySqlX.XDevAPI.Relational;

using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace DataAssetManager.DataApiServer.Web.Core
{
    public class RateLimitMiddleware
    {
        private readonly IDistributedCacheService _cache;
        private readonly RequestDelegate _next;
        private readonly DataApiServer.Core.RateLimiter _rateLimiter;
        private readonly IDataApiLogService _apiLogService;
        private readonly IAssetClientsService _assetClientsService;

        public RateLimitMiddleware(RequestDelegate next, IDistributedCacheService cache, DataApiServer.Core.RateLimiter rateLimiter,
            IAssetClientsService assetClientsService,
        IDataApiLogService dataApiLogService)
        {
            _next = next;
            _cache = cache;
            _rateLimiter = rateLimiter;
            _apiLogService = dataApiLogService;
            _assetClientsService = assetClientsService;
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
            if (routInfo == null)
            {
                routInfo = new RouteInfo()
                {
                    Id = path,
                    ApiUrl = path,
                    ApiName = path,
                    RateLimit = new RateLimit()
                };
            }
            var limit = await _assetClientsService.GetClientScopesByClientId(context.Request.Headers["x-token"], routInfo.TableId);
            if (limit != null && limit.ConfigRule != null)
            {
                if (routInfo.RateLimit.times < limit.ConfigRule.Times)
                    routInfo.RateLimit.times = limit.ConfigRule.Times;
                if (routInfo.RateLimit.seconds > limit.ConfigRule.Seconds && limit.ConfigRule.Seconds > 0)
                    routInfo.RateLimit.seconds = limit.ConfigRule.Seconds;
            }
            if (!_rateLimiter.IsAllowed(context, routInfo.Id, routInfo.RateLimit))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(new RESTfulResult<string>() { Errors = $"Too many requests. More then {routInfo.RateLimit.times} in {routInfo.RateLimit.seconds} Seconds. Please try again later." });

                //记录api调用情况
                sw.Stop();
                var _ = _apiLogService.SendLogEvent(context, routInfo, (int)sw.ElapsedMilliseconds, $"Too many requests. HTTP status code 429.API{routInfo.ApiName}({routInfo.Id})");
                return;
            }
            sw.Stop();
            await _next(context);
        }
    }
}