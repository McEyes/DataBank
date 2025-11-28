using ITPortal.Core.Services;

using SqlSugar;

using System;

namespace ITPortal.Search.Core.Models
{
    [SugarTable("SearchTopicGrants")]
    public class SearchTopicGrantsEntity : Entity<Guid>
    {

        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; set; }
        public string RoleId { get; set; }

        public Guid TopicId { get; set; }

        public DateTimeOffset CreateTime { get; set; }

    }
}
