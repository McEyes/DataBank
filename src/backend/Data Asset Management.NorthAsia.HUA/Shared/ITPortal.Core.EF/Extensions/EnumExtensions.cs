using Elastic.Clients.Elasticsearch.Fluent;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Extensions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Extensions
{
    public static class EnumExtensions
    {
        //// Cache to store display names of enum values
        //private static readonly ConcurrentDictionary<Enum, WereTypeInfoAttribute> WereTypeInfoCache = new();
        //private static Dictionary<string, WhereType> whereTypeMap = new Dictionary<string, WhereType>(){
        //    {"eq", WhereType.EQUALS},
        //    {"ne", WhereType.NOT_EQUALS},
        //    {"like", WhereType.LIKE },
        //    {"likel", WhereType.LIKE_LEFT },
        //    {"liker", WhereType.LIKE_RIGHT },
        //    {"gt", WhereType.GREATER_THAN},
        //    {"ge", WhereType.GREATER_THAN_EQUALS},
        //    {"lt", WhereType.LESS_THAN},
        //    {"le", WhereType.LESS_THAN_EQUALS},
        //    {"in", WhereType.IN},
        //    {"between", WhereType.BETWEEN},
        //    {"null", WhereType.NULL},
        //    {"notnull", WhereType.NOT_NULL},
        //    {"0", WhereType.None},
        //};

        public static T GetEnum<T>(this string data) where T : struct
        {
            foreach (T type in Enum.GetValues(typeof(T)))
            {
                if (type.ToString() == data || Convert.ToInt32(type).ToString() == data)
                {
                    return type;
                }
            }
            return default;
        }
        // Cache to store display names of enum values
        private static readonly ConcurrentDictionary<Enum, string> DescriptionCache = new();
        /// <summary>
        /// Gets the enum display name.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>
        /// Use <see cref="DisplayAttribute"/> if it exists.
        /// Otherwise, use the standard string representation.
        /// </returns>
        public static string GetDescription(this Enum enumValue)
        {
            // Retrieve the display name from the cache if it exists
            return DescriptionCache.GetOrAdd(enumValue, e =>
            {
                // Get the DisplayAttribute
                var attribute = e.GetAttributeOfType<DescriptionAttribute>();

                // Return the DisplayAttribute name if it exists, otherwise return the enum's string representation
                return attribute == null ? e.ToString() : attribute.Description;
            });
        }
        /// <summary>
        /// 将枚举值转换为整数
        /// </summary>
        /// <param name="dbType">枚举值</param>
        /// <returns>对应的整数值</returns>
        public static int ToInt(this Enum edata)
        {
            return Convert.ToInt32(edata);
        }

        ///// <summary>
        ///// Gets the enum display name.
        ///// </summary>
        ///// <param name="enumValue">The enum value.</param>
        ///// <returns>
        ///// Use <see cref="DisplayAttribute"/> if it exists.
        ///// Otherwise, use the standard string representation.
        ///// </returns>
        //public static WereTypeInfoAttribute GetWereTypeInfo(this Enum enumValue)
        //{
        //    // Retrieve the display name from the cache if it exists
        //    return WereTypeInfoCache.GetOrAdd(enumValue, e =>
        //    {
        //        // Get the DisplayAttribute
        //        var attribute = e.GetAttributeOfType<WereTypeInfoAttribute>();

        //        // Return the DisplayAttribute name if it exists, otherwise return the enum's string representation
        //        return attribute == null ? new WereTypeInfoAttribute("", "", "", "") : attribute;
        //    });
        //}

        //public static WereTypeInfoAttribute GetWereTypeInfo(this string whereType)
        //{
        //    if (whereTypeMap.ContainsKey(whereType)) return whereTypeMap[whereType].GetWereTypeInfo();
        //    else return WhereType.None.GetWereTypeInfo();
        //}

        //public static WhereType GetWereType(this string whereType)
        //{
        //    return whereTypeMap[whereType];
        //}
    }
}
