using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.LightElasticSearch
{
    /// <summary>
    /// 字段映射类型
    /// </summary>
    public class ElasticFieldAttribute : Attribute
    {

        private static readonly ConcurrentDictionary<Type, ElasticFieldAttribute> CachedTypeLookups = new ConcurrentDictionary<Type, ElasticFieldAttribute>();
        public bool IsKeyword { get; set; }
        public bool IsId { get; set; }

        //
        // 摘要:
        //     The name of the CLR type for serialization
        public string RelationName { get; set; }


        public ElasticFieldAttribute(bool isKeyword = false, bool isId = false)
        {
            IsKeyword = isKeyword;
            IsId = isId;
        }

        //
        // 摘要:
        //     Gets the first Nest.ElasticsearchTypeAttribute from a given CLR type
        public static ElasticFieldAttribute From(Type type)
        {
            if (CachedTypeLookups.TryGetValue(type, out var value))
            {
                return value;
            }

            object[] customAttributes = type.GetCustomAttributes(typeof(ElasticFieldAttribute), inherit: true);
            if (customAttributes.Any())
            {
                value = (ElasticFieldAttribute)customAttributes.First();
            }

            CachedTypeLookups.TryAdd(type, value);
            return value;
        }
    }   
}
