
using ITPortal.Extension.System;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITPortal.Core.DateFormatExtensions
{
    // DateTime 转换器
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format;
        private readonly DateTime _defaultValue = DateTime.MinValue;

        public CustomDateTimeConverter(string format = "yyyy-MM-dd HH:mm:ss")
        {
            _format = format;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String &&
                DateTime.TryParse(reader.GetString(), out var date))
            {
                return date;
            }
            return _defaultValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value == _defaultValue)
            {
                writer.WriteStringValue("");
                return;
            }
            writer.WriteStringValue(value.ToString(_format));
        }
    }

    // DateTimeOffset 转换器
    public class CustomDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        private readonly string _format;
        private readonly DateTimeOffset _defaultValue = DateTimeOffset.MinValue;
        //private readonly TimeZoneInfo _timeZone;

        public CustomDateTimeOffsetConverter(
            string format = "yyyy-MM-dd HH:mm:ss")
        {
            _format = format;
            //_timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateStr = reader.GetString();
            if (dateStr.IsNotNullOrWhiteSpace() && !dateStr.EndsWith("Z") && !dateStr.EndsWith("z"))
                dateStr = dateStr + "Z";
            if (reader.TokenType == JsonTokenType.String &&
                DateTimeOffset.TryParse(dateStr, out var date))
            {
                return date;
            }
            return _defaultValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            if (value == _defaultValue)
            {
                writer.WriteStringValue("");
                return;
            }
            writer.WriteStringValue(value.ToLocalTime().ToString(_format));
        }
    }

}
