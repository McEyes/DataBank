using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;

using ITPortal.Extension.System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.LightElasticSearch
{
    /// <summary>
    /// 配置Index索引，以_号分隔，系统模块
    /// 命名规则：系统/主题_索引名称；系统/主题_子系统/子主题_索引名称；
    /// </summary>
    public class ElasticIndexNameAttribute : Attribute
    {
        public string IdProperty { get; set; } = "Id";
        /// <summary>
        /// 创建索引时使用的名称
        /// 命名规则：系统/主题_索引名称；系统/主题_子系统/子主题_索引名称；
        /// Index主题域路径，以_号分隔，如请求路径为：IT_DataBase_Api,表示创建IT主题，DataBase子主题，下面的Api索引
        /// </summary>
        public string IndexFullName { get; private set; }

        /// <summary>
        /// 搜索时使用的别名
        /// 命名规则：系统/主题_索引名称；系统/主题_子系统/子主题_索引名称；
        /// Index主题域路径，以_号分隔，如请求路径为：IT_DataBase_Api,表示创建IT主题，DataBase子主题，下面的Api索引
        /// </summary>
        public string IndexAliasName { get; private set; }

        public IndexShardLifecycle Lifecycle { get; private set; } = IndexShardLifecycle.Day;
        ///// <summary>
        ///// 所属主题域
        ///// </summary>
        //private string Topic { get;  set; }
        ///// <summary>
        ///// 所属子主题域
        ///// </summary>
        //private string SubTopic { get;  set; }
        ///// <summary>
        ///// 索引名称，默认类名
        ///// </summary>
        //private string IndexName { get;  set; }

        public ElasticIndexNameAttribute(string indexName, string topic, string subTopic = "",string idProperty="Id")
        {
            if (indexName.IsNullOrWhiteSpace()) throw new ArgumentNullException("IndexName is not null or empty");
            if (topic.IsNullOrWhiteSpace()) throw new ArgumentNullException("topic is not null or empty");
            indexName = indexName.ToLower();
            topic = topic.ToLower();
            subTopic = subTopic.ToLower();
            IndexFullName = GetFullName(indexName, topic, subTopic);
            IndexAliasName = AliasName(indexName, topic);
            Lifecycle = IndexShardLifecycle.Month;
            IdProperty = idProperty;
        }

        public ElasticIndexNameAttribute(string indexName, string topic, IndexShardLifecycle lifecycle, string idProperty = "Id")
        {
            if (indexName.IsNullOrWhiteSpace()) throw new ArgumentNullException("IndexName is not null or empty");
            if (topic.IsNullOrWhiteSpace()) throw new ArgumentNullException("topic is not null or empty");
            indexName = indexName.ToLower();
            topic = topic.ToLower();
            IndexFullName = GetFullName(indexName, topic);
            IndexAliasName = AliasName(indexName, topic);
            Lifecycle = lifecycle;
            IdProperty = idProperty;
        }


        public ElasticIndexNameAttribute(string indexFullName, IndexShardLifecycle lifecycle = IndexShardLifecycle.Month, string idProperty = "Id")
        {
            if (indexFullName.IsNullOrWhiteSpace()) throw new ArgumentNullException("IndexFullName is not null or empty");
            IndexFullName = indexFullName.ToLower();
            Lifecycle = lifecycle;
            IdProperty = idProperty;
        }


        private string GetFullName(string indexName, string topic, string subTopic = "")
        {
            IndexAliasName = indexName;
            if (!topic.IsNullOrWhiteSpace())
            {
                if (!subTopic.IsNullOrWhiteSpace()) return $"{topic}_{subTopic}_{indexName}";
                IndexAliasName = $"{topic}_{indexName}";
            }
            if (IndexAliasName.IsNullOrWhiteSpace()) return IndexAliasName;
            switch (Lifecycle)
            {
                default:
                case IndexShardLifecycle.None:
                    return IndexAliasName;
                case IndexShardLifecycle.Month:
                    return $"{IndexAliasName}_{DateTimeOffset.Now.ToString("yyyyMM")}";
                case IndexShardLifecycle.Year:
                    return $"{IndexAliasName}_{DateTimeOffset.Now.ToString("yyyy")}";
                case IndexShardLifecycle.Day:
                    return $"{IndexAliasName}_{DateTimeOffset.Now.ToString("yyyyMMdd")}";
            }
        }

        private string AliasName(string indexName, string topic, string subTopic = "")
        {
            IndexAliasName = indexName;
            if (!topic.IsNullOrWhiteSpace())
            {
                if (!subTopic.IsNullOrWhiteSpace()) return $"{topic}_{subTopic}_{indexName}";
                IndexAliasName = $"{topic}_{indexName}*";
            }
            if (IndexAliasName.IsNullOrWhiteSpace()) return IndexAliasName;
            return IndexAliasName;
        }
    }

    public enum IndexShardLifecycle
    {
        /// <summary>
        /// 一个索引，数据基本上不变的，或者数据总量很小的，直接一个索引即可
        /// </summary>
        None = 0,
        /// <summary>
        /// 天，按天建立索引，如果数据流非常大，每天都是上G的情况，建议按天建立索引
        /// </summary>
        Day = 1,
        /// <summary>
        /// 月，按月建立索引，普通业务数据，每天数据流不多，但是也不少
        /// </summary>
        Month = 2,
        ////季度
        //Quarter = 3,
        ////半年
        //HalfYear = 4,
        /// <summary>
        /// 年，有数据会添加，但是数据增量很少
        /// </summary>
        Year = 5,

    }

}
