using Furion.EventBus;

using ITPortal.Core.LightElasticSearch;
using ITPortal.Extension.System;

using Newtonsoft.Json;

using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ITPortal.Core.EvenBus
{

    /// <summary>
    /// evenbus rids缓存
    /// </summary>
    public sealed class RedisEventSourceStorer : IEventSourceStorer, IDisposable
    {
        /// <summary>
        /// 内存通道事件源存储器
        /// </summary>
        private readonly Channel<IEventSource> _channel;

        /// <summary>
        /// Redis 连接对象
        /// </summary>
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        /// <summary>
        /// 路由键
        /// </summary>
        private readonly string _routeKey;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionMultiplexer">Redis 连接对象</param>
        /// <param name="routeKey">路由键</param>
        /// <param name="capacity">存储器最多能够处理多少消息，超过该容量进入等待写入</param>
        public RedisEventSourceStorer(ConnectionMultiplexer connectionMultiplexer, string routeKey, int capacity)
        {
            // 配置通道，设置超出默认容量后进入等待
            var boundedChannelOptions = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            // 创建有限容量通道
            _channel = Channel.CreateBounded<IEventSource>(boundedChannelOptions);

            _connectionMultiplexer = connectionMultiplexer;
            _routeKey = routeKey;

            // 获取一个订阅对象
            var subscriber = connectionMultiplexer.GetSubscriber();

            // 订阅消息
            subscriber.Subscribe(routeKey, (channel, data) =>
            {
                // 转换为 IEventSource，这里可以选择自己喜欢的序列化工具，如果自定义了 EventSource，注意属性是可读可写
                var eventSource = data.ToString().To<EventBusMessage>();//   JsonSerializer.Deserialize<ChannelEventSource>(data.ToString());

                // 写入内存管道存储器
                _channel.Writer.WriteAsync(eventSource.EventSource);
            });
        }

        /// <summary>
        /// 将事件源写入存储器
        /// </summary>
        /// <param name="eventSource">事件源对象</param>
        /// <param name="cancellationToken">取消任务 Token</param>
        /// <returns><see cref="ValueTask"/></returns>
        public async ValueTask WriteAsync(IEventSource eventSource, CancellationToken cancellationToken)
        {
            // 空检查
            if (eventSource == default)
            {
                throw new ArgumentNullException(nameof(eventSource));
            }
            if (eventSource is RedisEventSource)
            {
                // 序列化，这里可以选择自己喜欢的序列化工具
                var data = new EventBusMessage(eventSource).ToJSON();// JsonSerializer.Serialize(source);

                // 获取一个订阅对象
                var subscriber = _connectionMultiplexer.GetSubscriber();
                await subscriber.PublishAsync(_routeKey, data);
            }
            else
            {
                // 写入内存管道存储器
                await _channel.Writer.WriteAsync(eventSource);
            }
        }

        /// <summary>
        /// 从存储器中读取一条事件源
        /// </summary>
        /// <param name="cancellationToken">取消任务 Token</param>
        /// <returns>事件源对象</returns>
        public async ValueTask<IEventSource> ReadAsync(CancellationToken cancellationToken)
        {
            // 读取一条事件源
            var eventSource = await _channel.Reader.ReadAsync(cancellationToken);
            return eventSource;
        }

        /// <summary>
        /// 释放非托管资源
        /// </summary>
        public void Dispose()
        {
            _connectionMultiplexer.Dispose();
        }
    }
}
