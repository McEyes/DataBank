using ITPortal.Core.Services;

using SqlSugar;

using System;

namespace ITPortal.Search.Core.Models
{
    [SugarTable("UserSearchHistory")]
    public class UserSearchHistoryEntity : Entity<Guid>
    {

        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; set; }

        public string Keyword { get; set; }

        public string UserId { get; set; }

        public DateTimeOffset CreationTime { get; set; }

    }
}
