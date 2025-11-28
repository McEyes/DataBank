using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace ITPortal.Core.EvenBus
{
    public class EventBusOptions
    {
        /// <summary>
        /// EventBus 存储事件类型
        /// memory,redis
        /// </summary>
        public string StoreType { get; set; } = "memory";

        /// <summary>
        /// 路由键
        /// </summary>
        public string RouteKey { get; set; }

        /// <summary>
        /// 存储器最多能够处理多少消息，超过该容量进入等待写入，默认3000
        /// </summary>
        public int Capacity { get; set; } = 3000;

        /// <summary>
        /// redis 的配置路径，也可以直接配置RedisCacheOptions
        /// </summary>
        public string RedisConfigPath { get; set; }

        public RedisCacheOptions Redis { get; set; }

    }
}
