
using SqlSugar;

using System;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ITPortal.Core.Services
{
    public interface IEntity<KeyType>
    {
        KeyType Id { get; set; }

        ///// <summary>
        ///// CreateOrg
        ///// </summary>
        //[SugarColumn(ColumnName = "create_org")]
        //string? CreateOrg { get; set; }

        ///// <summary>
        ///// createTime
        ///// </summary>
        //[SugarColumn(ColumnName = "create_time")]
        //DateTimeOffset CreateTime { get; set; }
        ///// <summary>
        ///// createBy
        ///// </summary>
        //[SugarColumn(ColumnName = "create_by")]
        //string CreateBy { get; set; }

        ///// <summary>
        ///// updateTime
        ///// </summary>
        //[SugarColumn(ColumnName = "update_time")]
        //DateTimeOffset? UpdateTime { get; set; }
        ///// <summary>
        ///// updateBy
        ///// </summary>
        //[SugarColumn(ColumnName = "update_by")]
        //string? UpdateBy { get; set; }
    }

    public interface ICreateEntity<KeyType> : IEntity<KeyType>
    {
        /// <summary>
        /// createTime
        /// </summary>
        [SugarColumn(ColumnName = "create_time", IsOnlyIgnoreUpdate = true)]
        DateTimeOffset CreateTime { get; set; }
        /// <summary>
        /// createBy
        /// </summary>
        [SugarColumn(ColumnName = "create_by", IsOnlyIgnoreUpdate = true)]
        string CreateBy { get; set; }
    }

    public interface IAuditEntity<KeyType> : ICreateEntity<KeyType>
    {

        /// <summary>
        /// updateTime
        /// </summary>
        [SugarColumn(ColumnName = "update_time")]
        DateTimeOffset? UpdateTime { get; set; }
        /// <summary>
        /// updateBy
        /// </summary>
        [SugarColumn(ColumnName = "update_by")]
        string? UpdateBy { get; set; }
    }


    public interface ISoftDeleteEntity<KeyType>
    {
        [SugarColumn(ColumnName = "is_delete")]
        bool? IsDelete { get; set; }

        /// <summary>
        /// updateTime
        /// </summary>
        [SugarColumn(ColumnName = "delete_time")]
        DateTimeOffset? DeleteTime { get; set; }
    }

    public interface IStatusEntity
    {
        /// <summary>
        /// status 0-unuse 1-use
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        int? Status { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        [SugarColumn(ColumnName = "remark")]
        string? Remark { get; set; }
    }




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
        public virtual DateTimeOffset CreateTime { get; set; }
        /// <summary>
        /// createBy
        /// </summary>
        [SugarColumn(ColumnName = "create_by")]
        public virtual string CreateBy { get; set; }
    }

    public class AuditEntity<KeyType> : CreateEntity<KeyType> ,IAuditEntity<KeyType>
    {
        /// <summary>
        /// updateTime
        /// </summary>
        [SugarColumn(ColumnName = "update_time")]
        public virtual DateTimeOffset? UpdateTime { get; set; }
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


    public interface IAuditNameEntity
    {
        [SugarColumn(ColumnName = "create_by_name")]
        public string? CreatedByName { get; set; }
        [SugarColumn(ColumnName = "update_by_name")]
        public string? UpdateByName { get; set; }
    }


    public interface IPageEntity<KeyType> : IEntity<KeyType>
    {
        int PageNum { get; set; }
        int PageSize { get; set; }
        int SkipCount { get;}
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
        public string? OrderField { get; set; }
        //[SugarColumn(IsIgnore = true)]
        //public List<OrderItem>? OrderList { get; set; }
        //[SugarColumn(IsIgnore = true)]
        //public string? DataScope { get; set; }

    }


    //public class OrderItem
    //{
    //    private string column;
    //    private bool asc;
    //}
}
