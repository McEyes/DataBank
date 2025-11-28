using Newtonsoft.Json;
namespace ITPortal.Extension.System.DateFormatExtensions
{
    public class DefaultDateConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime DefaultDateTime = DateTime.MinValue;
        private readonly string _format;

        public DefaultDateConverter(string format = "yyyy-MM-dd HH:mm:ss.fffffff")
        {
            _format = format;
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var str = reader.Value?.ToString();
                if (string.IsNullOrEmpty(str))
                    return DefaultDateTime;

                if (DateTime.TryParse(str, out var date))
                    return date;
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            if (value == DefaultDateTime)
            {
                writer.WriteValue((new Nullable<DateTime>()).ToString());
            }
            else
            {
                writer.WriteValue(value.ToString(_format));
            }
        }
    }

    public class DefaultDate2Converter : JsonConverter<DateTime?>
    {
        private static readonly DateTime? DefaultDateTime = null;
        private readonly string _format;

        public DefaultDate2Converter(string format = "yyyy-MM-dd HH:mm:ss.fffffff")
        {
            _format = format;
        }

        public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var str = reader.Value?.ToString();
                if (string.IsNullOrEmpty(str))
                    return DefaultDateTime;

                if (DateTime.TryParse(str, out var date))
                    return date;
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
        {
            if (value == DefaultDateTime || value == DateTime.MinValue)
            {
                writer.WriteValue((new Nullable<DateTime>()).ToString());
            }
            else
            {
                writer.WriteValue(value.Value.ToString(_format));
            }
        }
    }
    public class DefaultDateOnlyConverter : JsonConverter<DateOnly>
    {
        private static readonly DateOnly DefaultDateTime = DateOnly.MinValue;
        private readonly string _format;

        public DefaultDateOnlyConverter(string format = "yyyy-MM-dd")
        {
            _format = format;
        }

        public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var str = reader.Value?.ToString();
                if (string.IsNullOrEmpty(str))
                    return DefaultDateTime;

                if (DateOnly.TryParse(str, out var date))
                    return date;
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
        {
            if (value == DefaultDateTime)
            {
                writer.WriteValue((new Nullable<DateOnly>()).ToString());
            }
            else
            {
                writer.WriteValue(value.ToString(_format));
            }
        }
    }

    public class UtcDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        private readonly string _format;
        private readonly TimeZoneInfo _timeZone;

        public UtcDateTimeOffsetConverter(string format = "yyyy-MM-dd HH:mm:ss.fffffff zzz",
                                         string timeZoneId = "UTC")
        {
            _format = format;
            _timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        public override DateTimeOffset ReadJson(JsonReader reader, Type objectType,
                                               DateTimeOffset existingValue,
                                               bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || reader.Value.ToString().IsNullOrWhiteSpace())
                return existingValue;
            var dateStr = reader.Value.ToString();
            if (!(dateStr.EndsWith("z", StringComparison.CurrentCultureIgnoreCase) || dateStr.Contains(" +"))) dateStr += " +08:00";
            if (reader.TokenType == JsonToken.String &&
                DateTimeOffset.TryParse(dateStr, out var date))
            {
                return date;
            }
            else if (reader.Value is DateTime)
            {
                return new DateTimeOffset((DateTime)reader.Value);
            }
            else if (reader.Value is DateTimeOffset)
            {
                return (DateTimeOffset)reader.Value;
            }
            return existingValue;


            //if (reader.Value == null || reader.Value.ToString().IsNullOrWhiteSpace())
            //    return DateTimeOffset.MinValue;
            //DateTimeOffset dateTimeOffset = existingValue;
            //// 解析字符串为DateTimeOffset，假设输入为UTC时间
            //if (reader.Value is DateTime)
            //{
            //    dateTimeOffset = new DateTimeOffset((DateTime)reader.Value);
            //}
            //else if (reader.Value is DateTimeOffset)
            //{
            //    dateTimeOffset = (DateTimeOffset)reader.Value;
            //}
            //else
            //{
            //    var dateStr = reader.Value.ToString();
            //    if (!(dateStr.EndsWith("z", StringComparison.CurrentCultureIgnoreCase) || dateStr.Contains(" +"))) dateStr += " +08:00";
            //    dateTimeOffset = DateTimeOffset.Parse(dateStr);
            //}
            //return dateTimeOffset;

            //var time2 = dateTimeOffset.ToLocalTime().ToString(_format);
            //if (dateTimeOffset.Offset.Ticks == 0)
            //{
            //    dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromHours(8));
            //    dateTimeOffset = dateTimeOffset.AddHours(-8);
            //}
            // 转换为东8区时间
            //DateTimeOffset chinaDateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromHours(8));
            //// 解析字符串，得到未指定时区的DateTime
            //DateTime dateTime = DateTime.Parse(reader.Value.ToString());
            //// 获取东8区（中国标准时间）
            //TimeZoneInfo chinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

            //// 将未指定时区的DateTime转换为东8区时间
            //DateTime chinaDateTime = TimeZoneInfo.ConvertTime(dateTime, chinaTimeZone);
        }

        public override void WriteJson(JsonWriter writer, DateTimeOffset value,
                                      JsonSerializer serializer)
        {
            // 转换为目标时区
            //var convertedTime = TimeZoneInfo.ConvertTime(value, _timeZone);

            // 处理默认值
            if (value == DateTimeOffset.MinValue)
            {
                writer.WriteValue(new Nullable<DateTimeOffset>());
                return;
            }

            writer.WriteValue(value.ToLocalTime().ToString(_format));
        }
    }

    public class UtcDateTimeOffset2Converter : JsonConverter<DateTimeOffset?>
    {
        private readonly string _format;

        public UtcDateTimeOffset2Converter(string format = "yyyy-MM-dd HH:mm:ss.fffffff zzz")
        {
            _format = format;
        }

        public override DateTimeOffset? ReadJson(JsonReader reader, Type objectType,
                                               DateTimeOffset? existingValue,
                                               bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || reader.Value.ToString().IsNullOrWhiteSpace())
                return null;
            var dateStr = reader.Value.ToString();
            if (!(dateStr.EndsWith("z", StringComparison.CurrentCultureIgnoreCase) || dateStr.Contains(" +"))) dateStr += " +08:00";
            if (reader.TokenType == JsonToken.String &&
                DateTimeOffset.TryParse(dateStr, out var date))
            {
                return date;
            }
            else if (reader.Value is DateTime)
            {
                return new DateTimeOffset((DateTime)reader.Value);
            }
            else if (reader.Value is DateTimeOffset)
            {
                return (DateTimeOffset)reader.Value;
            }
            return existingValue;


            //if (reader.Value == null || reader.Value.ToString().IsNullOrWhiteSpace())
            //    return null;
            //reader.TokenType == JsonTokenType.String &&
            //DateTimeOffset ? dateTimeOffset = existingValue;
            //if (reader.Value is DateTime)
            //{
            //    dateTimeOffset = new DateTimeOffset((DateTime)reader.Value);
            //}
            //else if (reader.Value is DateTimeOffset)
            //{
            //    dateTimeOffset = (DateTimeOffset)reader.Value;
            //}
            //else
            //{
            //    // 解析字符串为DateTimeOffset，假设输入为UTC时间
            //    var dateStr = reader.Value.ToString();
            //    if (!(dateStr.EndsWith("z", StringComparison.CurrentCultureIgnoreCase) || dateStr.Contains(" +"))) dateStr += " +08:00";
            //    dateTimeOffset = DateTimeOffset.Parse(dateStr);
            //}
            //return dateTimeOffset;
        }

        public override void WriteJson(JsonWriter writer, DateTimeOffset? value,
                                      JsonSerializer serializer)
        {
            // 处理默认值
            if (value == null || value == DateTimeOffset.MinValue)
            {
                writer.WriteValue(value);
                return;
            }
            // 转换为目标时区
            //var convertedTime = TimeZoneInfo.ConvertTime(value.Value, _timeZone);

            writer.WriteValue(value.Value.ToLocalTime().ToString(_format));
        }
    }
}
