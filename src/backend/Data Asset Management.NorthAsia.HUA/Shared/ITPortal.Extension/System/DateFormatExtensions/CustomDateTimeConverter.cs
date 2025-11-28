using ITPortal.Extension.System;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITPortal.Extension.System.DateFormatExtensions
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
    // DateTime 转换器
    public class CustomDateTime2Converter : JsonConverter<DateTime?>
    {
        private readonly string _format;
        private readonly DateTime? _defaultValue = null;

        public CustomDateTime2Converter(string format = "yyyy-MM-dd HH:mm:ss")
        {
            _format = format;
        }

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String &&
                DateTime.TryParse(reader.GetString(), out var date))
            {
                return date;
            }
            return _defaultValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == _defaultValue || value == DateTime.MinValue)
            {
                writer.WriteStringValue("");
                return;
            }
            writer.WriteStringValue(value.Value.ToString(_format));
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
            if (!(dateStr.EndsWith("z", StringComparison.CurrentCultureIgnoreCase) || dateStr.Contains(" +"))) dateStr += " +08:00";
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

    // DateTimeOffset 转换器
    public class CustomDateTimeOffset2Converter : JsonConverter<DateTimeOffset?>
    {
        private readonly string _format;
        private readonly DateTimeOffset? _defaultValue = null;
        //private readonly TimeZoneInfo _timeZone;

        public CustomDateTimeOffset2Converter(
            string format = "yyyy-MM-dd HH:mm:ss")
        {
            _format = format;
            //_timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateStr = reader.GetString();
            if (!(dateStr.EndsWith("z", StringComparison.CurrentCultureIgnoreCase) || dateStr.Contains(" +"))) dateStr += " +08:00";
            if (reader.TokenType == JsonTokenType.String &&
                DateTimeOffset.TryParse(dateStr, out var date))
            {
                return date;
            }
            return _defaultValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            if (value == _defaultValue || value == DateTimeOffset.MinValue)
            {
                writer.WriteStringValue("");
                return;
            }
            writer.WriteStringValue(value.Value.ToLocalTime().ToString(_format));
        }
    }
}
