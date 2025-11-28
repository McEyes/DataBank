using Furion.EventBus;

using ITPortal.Extension.System;

using Newtonsoft.Json;

namespace ITPortal.Core.EvenBus
{
    public class EventBusMessage
    {

        public EventBusMessage()
        {
        }

        public EventBusMessage(IEventSource dataSource)
        {
            ModelTypeName = dataSource.GetType().AssemblyQualifiedName;
            Payload = dataSource.ToJSON();
        }

        public EventBusMessage(Type modelType, string payload)
        {
            ModelTypeName = modelType.AssemblyQualifiedName;
            Payload = payload;
        }

        public string ModelTypeName { get; set; }
        private Type _ModelType;
        [JsonIgnore]
        public Type ModelType
        {
            get
            {
                if (_ModelType == null) _ModelType = Type.GetType(ModelTypeName);
                return _ModelType;
            }
        }
        private IEventSource _eventSource;

        [JsonIgnore]
        public IEventSource EventSource
        {
            get
            {
                if (_eventSource == null) _eventSource = (IEventSource)Payload.FromJsonString(ModelType);
                return _eventSource;
            }
        }
        /// <summary>
        /// IEventSourceµÄJsonÊý¾Ý
        /// </summary>
        public string Payload { get; set; }
    }
}
