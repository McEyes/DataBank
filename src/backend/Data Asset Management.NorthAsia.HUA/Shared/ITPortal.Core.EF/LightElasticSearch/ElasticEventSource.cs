using Furion.EventBus;

namespace ITPortal.Core.LightElasticSearch
{
    public class ElasticEventSource : IEventSource
    {
        /// <summary>
        /// 
        /// </summary>
        public ElasticEventSource()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="indexType"></param>
        /// <param name="data"></param>
        public ElasticEventSource(string eventId,Type indexType,object data)
        {
            EventId = eventId;
            IndexType = indexType;
            Payload = data;
        }

        public string EventId { get; set; }
        public string IndexName { get; set; }
        public Type IndexType { get; set; }

        public object Payload { get; set; }

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
