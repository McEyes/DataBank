using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Models
{
    public static class SwaggerDataFormat
    {
        public static readonly Dictionary<string, List<string>> DataFmts = new()
        {
            { "string",new List<string>{ "date", "date-time", "email", "uuid", "uri","" } },
            { "integer",new List<string>{ "int32", "int64" } },
            { "number",new List<string>{ "float", "double" } },

            { "array",new List<string>{ "float", "double", "int32", "int64", "string" } }
        };

        public const string data_format_int32 = "int32";
        public const string data_format_int64 = "int64";
        public const string data_format_float = "float";
        public const string data_format_double = "double";
        public const string data_format_date = "date";
        public const string data_format_date_time = "date-time";
        public const string data_format_email = "email";
        public const string data_format_uuid = "uuid";
        public const string data_format_uri = "uri";
    }
}
