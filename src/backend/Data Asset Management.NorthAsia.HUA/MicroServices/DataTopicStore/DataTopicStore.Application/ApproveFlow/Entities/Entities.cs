using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.ApproveFlow.Entities
{
    public class Entity<KeyType> : IEntity<KeyType>
    {
        public virtual KeyType Id { get; set; }

        //[SugarColumn(ColumnName = "create_org")]
        //public virtual string? CreateOrg { get; set; }
    }



    public class CreateEntity<KeyType> : Entity<KeyType>, ICreateEntity<KeyType>
    {

        /// <summary>
        /// createTime
        /// </summary>
        [SugarColumn(ColumnName = "create_time")]
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// createBy
        /// </summary>
        [SugarColumn(ColumnName = "create_by")]
        public virtual string CreateBy { get; set; }
    }

    public class AuditEntity<KeyType> : CreateEntity<KeyType>, IAuditEntity<KeyType>
    {
        /// <summary>
        /// updateTime
        /// </summary>
        [SugarColumn(ColumnName = "update_time")]
        public virtual DateTime? UpdateTime { get; set; }
        /// <summary>
        /// updateBy
        /// </summary>
        [SugarColumn(ColumnName = "update_by")]
        public virtual string? UpdateBy { get; set; }
    }


    public interface ICreateNameEntity
    {
        public string CreatedByName { get; set; }
    }



    public interface IPageEntity<KeyType> : IEntity<KeyType>
    {
        int PageNum { get; set; }
        int PageSize { get; set; }
        int SkipCount { get; }
    }

    public class PageEntity<KeyType> : Entity<KeyType>, IPageEntity<KeyType>
    {

        [SugarColumn(IsIgnore = true)]
        public int PageNum { get; set; } = 1;

        [SugarColumn(IsIgnore = true)]
        public int PageSize { get; set; } = 20;

        [SugarColumn(IsIgnore = true)]
        public int SkipCount
        {
            get
            {
                if (PageNum <= 0) PageNum = 1;
                if (PageSize <= 0) PageSize = 20;
                return (PageNum - 1) * PageSize;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string? Keyword { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<OrderItem>? OrderList { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string? DataScope { get; set; }

    }

    public class OrderItem
    {
        private string column;
        private bool asc;
    }
}
