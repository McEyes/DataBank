using ITPortal.Core.LightElasticSearch;

using Nest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Core.Models.Search
{
    [ElasticsearchType(RelationName = ElasticsearchRelationNames.TopicDocuments, IdProperty = "Id")]
    [ElasticIndexName(ElasticsearchRelationNames.TopicDocuments, IdProperty = "Id")]
    public class TopicDocumentModel
    {

        [Keyword]
        public string Id { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        [Keyword]
        public string Topic { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Text(Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Text(Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
        public string Description { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        [Completion(Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
        public List<string> Keyword { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [Keyword]
        public List<string> Classify { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [Text(Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
        public List<string> Tags { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Text(Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
        public string Content { get; set; }

        /// <summary>
        /// 额外数据
        /// </summary>
        [Keyword]
        public string PayloadStr { get; set; } = "";

        /// <summary>
        /// 关联URL
        /// </summary>
        [Keyword]
        public string linkUrl { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<TopicDocumentAttachmentsModel> Attachments { get; set; } = new List<TopicDocumentAttachmentsModel>();

        /// <summary>
        /// 创建人
        /// </summary>
        [Keyword]
        public string Creator { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [Keyword]
        public string Updater { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Date]
        public DateTimeOffset? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Date]
        public DateTimeOffset? UpdateTime { get; set; }

        /// <summary>
        /// 开启数据主权
        /// </summary>
        [Boolean]
        public bool? EnableDataSovereignty { get; set; }

        /// <summary>
        /// 高亮字典
        /// </summary>
        [Ignore]
        public Dictionary<string, string> Highlight { get; set; } = new Dictionary<string, string>();

    }
}
