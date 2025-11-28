using Furion.EventBus;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Text.Json;

namespace ITPortal.Core.EvenBus
{
    public static class EventBusExtensions
    {

        /// <summary>
        /// 将JSON字符串还原为对象
        /// </summary>
        /// <typeparam name="T">要转换的目标类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns></returns>
        public static T GetPayload<T>(this IEventSource source)
        {
            if (source == null || source.Payload == null) return default(T);
            else if (source.Payload is JObject|| source.Payload is JsonElement) return JsonConvert.DeserializeObject<T>(source.Payload.ToString());
            else return (T)source.Payload;
        }
        /// <summary>
        /// 将JSON字符串还原为对象
        /// </summary>
        /// <typeparam name="T">要转换的目标类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns></returns>
        public static T GetSourcePayload<T>(this EventHandlerExecutingContext context)
        {
            if (context == null || context.Source == null || context.Source.Payload == null) return default(T);
            else if (context.Source.Payload is JObject || context.Source.Payload is JsonElement) return JsonConvert.DeserializeObject<T>(context.Source.Payload.ToString());
            else return (T)context.Source.Payload;
        }
    }
}
