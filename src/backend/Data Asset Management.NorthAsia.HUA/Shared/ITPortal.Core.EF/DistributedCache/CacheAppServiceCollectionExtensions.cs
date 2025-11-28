using ITPortal.Extension.System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.DistributedCache
{
    /// <summary>
  /// 缓存类型枚举
  /// </summary>
    public enum CacheType
    {
        Memory,
        Redis
    }
    public static class CacheAppServiceCollectionExtensions
    {
        /// <summary>
        /// 根据配置注册缓存服务
        /// </summary>
        public static IServiceCollection AddCacheManager(this IServiceCollection services, IConfiguration configuration)
        {
            var cacheConfig = configuration.GetSection("Redis");
            var cacheType = cacheConfig["Configuration"].IsNullOrWhiteSpace() ? "memory" : "redis";

            switch (cacheType)
            {
                case "redis":
                default:
                    services.AddSingleton<IDistributedCacheService, RedisCacheService>();
                    break;
                case "memory":
                    services.AddMemoryCache();
                    services.AddSingleton<IDistributedCacheService, MemoryCacheService>();
                    break;
            }

            return services;
        }
    }
}
