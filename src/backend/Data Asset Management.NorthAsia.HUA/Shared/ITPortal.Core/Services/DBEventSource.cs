using Furion.EventBus;

namespace ITPortal.Core.LightElasticSearch
{
    public class DBEventSource : RedisEventSource
    {
        /// <summary>
        /// 
        /// </summary>
        public DBEventSource()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="modelType"></param>
        /// <param name="data"></param>
        public DBEventSource(string eventId, Type modelType, object data) : base(eventId, modelType, data)
        {
        }

    }
}
