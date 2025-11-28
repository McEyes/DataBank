using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dm.util;

namespace DataTopicStore.Web.Core
{
    public class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                // 处理 null 值
                if (reader.TokenType == JsonTokenType.Null)
                    return default;

                // 处理字符串格式的日期
                if (reader.TokenType == JsonTokenType.String)
                {
                    string dateString = reader.GetString();
                    if (DateTime.TryParseExact(dateString, Format, null, System.Globalization.DateTimeStyles.None, out DateTime date))
                    {
                        return date;
                    }
                    // 尝试其他格式
                    if (DateTime.TryParse(dateString, out date))
                    {
                        return date;
                    }
                }
                // 处理数字格式的时间戳（Unix时间戳）
                else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out long unixTime))
                {
                    return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
                }

                throw new JsonException($"无法将值转换为 DateTime。TokenType: {reader.TokenType}");
            }
            catch (Exception ex)
            {
                throw new JsonException("日期反序列化失败", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                writer.WriteStringValue(value.Value.ToString(Format));
            }

        }
    }
}
