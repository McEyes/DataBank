using Furion.EventBus;

namespace ITPortal.Core.LightElasticSearch
{
    public class RedisEventSource : IEventSource
    {
        /// <summary>
        /// 
        /// </summary>
        public RedisEventSource()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="modelType"></param>
        /// <param name="data"></param>
        public RedisEventSource(string eventId,Type modelType, object data)
        {
            EventId = eventId;
            ModelType = modelType;
            Payload = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        public RedisEventSource(string eventId)
        {
            EventId = eventId;
        }

        public string EventId { get; set; }
        public Type ModelType { get; set; }

        public object Payload { get; set; }
        ///// <summary>
        ///// 那台服务器,自动获取
        ///// </summary>
        //public string ServerHost { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 消息是否只消费一次
        /// </summary>
        public bool IsConsumOnce { get; set; } = true;
        /// <summary>
        /// 取消任务 Token
        /// </summary>
        /// <remarks>用于取消本次消息处理</remarks>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public CancellationToken CancellationToken { get; set; }
    }
}
