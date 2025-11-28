using Furion;

using ITPortal.Extension.System;

using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

namespace ITPortal.Core.EvenBus
{
    public static class AppServiceCollectionExtensions
    {
        public static IServiceCollection AddCabEventBus(this IServiceCollection services, Action<EventBusOptions> configure = null)
        {
            services.Configure<EventBusOptions>(option =>
            {
                if (configure != null)
                {
                    configure?.Invoke(option);
                }
                else
                {
                    var config = App.GetConfig<RedisCacheOptions>("Redis");
                    if (option.Redis == null) option.Redis = config;
                    else
                    {
                        option.Redis.Configuration = config.Configuration;
                        option.Redis.InstanceName = config.InstanceName;
                    }
                }
            });
            services.AddEventBus(options =>
            {
                var eventBusOptions = App.GetConfig<EventBusOptions>("EventBus");
                if (eventBusOptions == null) return;
                // 创建 Redis 连接对象
                if (eventBusOptions.Redis == null)
                {
                    eventBusOptions.Redis = App.GetConfig<RedisCacheOptions>("Redis");
                }
                if (eventBusOptions.Redis != null && eventBusOptions.Redis.Configuration.IsNotNullOrWhiteSpace())
                {
                    var connectionMultiplexer = ConnectionMultiplexer.Connect(eventBusOptions.Redis.Configuration);
                    // 创建默认内存通道事件源对象，可自定义队列路由key，比如这里是 eventbus
                    var redisEventSourceStorer = new RedisEventSourceStorer(connectionMultiplexer, $"{DataAssetManagerConst.RedisKey}{eventBusOptions.RouteKey}", 3000);
                    // 替换默认事件总线存储器
                    options.ReplaceStorer(serviceProvider =>
                    {
                        return redisEventSourceStorer;
                    });
                }
            });
            return services;
        }

    }
}
