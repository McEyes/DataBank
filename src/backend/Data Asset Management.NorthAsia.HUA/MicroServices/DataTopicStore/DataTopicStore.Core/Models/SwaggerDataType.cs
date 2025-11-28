using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Models
{
    public static class SwaggerDataType
    {
        //"string", "number", "integer", "boolean", "array", "object"
        public static string[] DataTypes = ["string", "number", "integer", "boolean", "array"];

        public const string output_type_object = "object";
        public const string output_type_array = "array";

        public const string data_type_string = "string";
        public const string data_type_number = "number";
        public const string data_type_integer = "integer";
        public const string data_type_boolean = "boolean";
        public const string data_type_array = "array";
        public const string data_type_object = "object";
    }
}
