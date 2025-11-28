using ITPortal.Core;
using ITPortal.Core.DistributedCache;

using Microsoft.Extensions.Logging;

using StackExchange.Profiling.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
namespace DataAssetManager.DataApiServer.Core
{
    public class RedisRateLimiter : ISingleton
    {
        private readonly IDistributedCacheService _cache;
        private readonly ILogger<RateLimiter> _logger;

        public RedisRateLimiter(IDistributedCacheService cache, ILogger<RateLimiter> logger)
        {
            _cache = cache;
            _logger= logger;
        }

        public bool IsAllowed(HttpContext context, string apiId, RateLimit limit)
        {
            if (limit.enable != "1")
                return true;
            string cacheKey = GetCacheKey(limit, apiId, GetClientIp(context.Request), GetCurrentUser(context));

            var currentTime = DateTimeOffset.Now;
            var accessRecords = _cache.GetObject(cacheKey, () => { return new List<DateTimeOffset>(); });
            accessRecords.RemoveAll(t => (currentTime - t).TotalSeconds > limit.seconds);

            // 检查访问次数是否超过限制
            if (accessRecords.Count >= limit.times)
            {
                return false;
            }

            // 添加当前访问记录
            accessRecords.Add(currentTime);
            _cache.SetObject(cacheKey, accessRecords, TimeSpan.FromSeconds(limit.seconds));
            return true;
        }

        public string GetCacheKey(RateLimit limit, string apiId, string ip, string userName)
        {
            StringBuilder cacheKey = new StringBuilder($"{ITPortal.Core.DataAssetManagerConst.RouteLimitRedisKey}.{apiId}");
            //if (limit.enableUserLimit)
            //    cacheKey.Append($":{userName}");
            //if (limit.enableIpLimit)
            //    cacheKey.Append($":{ip}");
            return cacheKey.ToString();
        }

        public string GetClientIp(HttpRequest request)
        {
            var headers = new List<string>() { "X-Forwarded-For", "Proxy-Client-IP", "WL-Proxy-Client-IP", "HTTP_CLIENT_IP", "HTTP_X_FORWARDED_FOR", "X-Forwarded-For" };
            string ip = GetAndCheckIP(request, headers.GetEnumerator());
            if (string.IsNullOrWhiteSpace(ip)) ip = request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString();
            if (ip.IsNullOrWhiteSpace()) ip = request.HttpContext.Connection.RemoteIpAddress?.ToString();
            return ip;
        }

        private string GetAndCheckIP(HttpRequest request, IEnumerator<string> headerNames)
        {
            while (headerNames.MoveNext())
            {
                var xffHeader = request.Headers[headerNames.Current].FirstOrDefault();
                if (!string.IsNullOrEmpty(xffHeader))
                {
                    var remoteIp = xffHeader.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    if (IPAddress.TryParse(remoteIp, out var ip))
                    {
                        return ip.ToString();
                    }
                }
            }
            return string.Empty;
        }

        public string GetCurrentUser(HttpContext context)
        {
            // 检查用户是否已通过身份验证
            if (context.User.Identity.IsAuthenticated)
            {
                return context.User.FindFirstValue(ClaimTypes.Name);
            }
            return string.Empty;
        }
    }
}