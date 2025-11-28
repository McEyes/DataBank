using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.Services;

using SqlSugar;

using System;


namespace ITPortal.Search.Core.Models
{
    [Serializable]
    [SugarTable("SearchTopic")]
    public class SearchTopicEntity : AuditEntity<Guid>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Topic
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否为公开主题
        /// </summary>
        public bool IsPublic { get; set; }

    }
}
