using ITPortal.Core;
using ITPortal.Core.WebApi;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Text;
namespace DataAssetManager.DataApiServer.Core
{
    public class RateLimiter:ISingleton
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<RateLimiter> _logger;

        public RateLimiter(IMemoryCache memoryCache, ILogger<RateLimiter> logger)
        {
            _memoryCache = memoryCache;
            _logger= logger;
        }

        public bool IsAllowed(HttpContext context, string apiId, RateLimit limit)
        {
            if (limit == null) limit = new RateLimit();
            if (limit.enable != "1")
                return true;
            string cacheKey = GetCacheKey(limit, apiId, IPAddressUtils.GetClientIp(context.Request), context.GetCurrUserInfo()?.NtId);

            var currentTime = DateTimeOffset.Now;
            var accessRecords = _memoryCache.Get<List<DateTimeOffset>>(cacheKey) ?? new List<DateTimeOffset>();
            accessRecords.RemoveAll(t => (currentTime - t).TotalSeconds > limit.seconds);

            // 检查访问次数是否超过限制
            if (accessRecords.Count >= limit.times)
            {
                return false;
            }

            // 添加当前访问记录
            accessRecords.Add(currentTime);
            _memoryCache.Set(cacheKey, accessRecords, TimeSpan.FromSeconds(limit.seconds));
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


    }
}