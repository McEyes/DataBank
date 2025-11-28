
using Furion.DatabaseAccessor;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ITPortal.Core.Services
{
    public interface IEntity<KeyType>
    {
        KeyType Id { get; set; }

        ///// <summary>
        ///// CreateOrg
        ///// </summary>
        //[Column( "create_org")]
        //string? CreateOrg { get; set; }

        ///// <summary>
        ///// createTime
        ///// </summary>
        //[Column( "create_time")]
        //DateTimeOffset CreateTime { get; set; }
        ///// <summary>
        ///// createBy
        ///// </summary>
        //[Column( "create_by")]
        //string CreateBy { get; set; }

        ///// <summary>
        ///// updateTime
        ///// </summary>
        //[Column( "update_time")]
        //DateTimeOffset? UpdateTime { get; set; }
        ///// <summary>
        ///// updateBy
        ///// </summary>
        //[Column( "update_by")]
        //string? UpdateBy { get; set; }
    }

    public interface ICreateEntity<KeyType> : IEntity<KeyType>
    {
        /// <summary>
        /// createTime
        /// </summary>
        [Column(name: "create_time")]
        DateTimeOffset CreateTime { get; set; }
        /// <summary>
        /// createBy
        /// </summary>
        [Column( "create_by")]
        string CreateBy { get; set; }
    }

    public interface IAuditEntity<KeyType> : ICreateEntity<KeyType>
    {

        /// <summary>
        /// updateTime
        /// </summary>
        [Column( "update_time")]
        DateTimeOffset? UpdateTime { get; set; }
        /// <summary>
        /// updateBy
        /// </summary>
        [Column( "update_by")]
        string? UpdateBy { get; set; }
    }


    public interface ISoftDeleteEntity<KeyType>
    {
        [Column( "is_delete")]
        bool? IsDelete { get; set; }

        /// <summary>
        /// updateTime
        /// </summary>
        [Column( "delete_time")]
        DateTimeOffset? DeleteTime { get; set; }
    }

    public interface IStatusEntity
    {
        /// <summary>
        /// status 0-unuse 1-use
        /// </summary>
        [Column( "status")]
        int? Status { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        [Column( "remark")]
        string? Remark { get; set; }
    }




    //public class Entity<KeyType> : IEntity<KeyType>
    //{
    //    public virtual KeyType Id { get; set; }

    //    //[Column( "create_org")]
    //    //public virtual string? CreateOrg { get; set; }
    //}



    //public class CreateEntity<KeyType> : Entity<KeyType>, ICreateEntity<KeyType>
    //{

    //    /// <summary>
    //    /// createTime
    //    /// </summary>
    //    [Column( "create_time")]
    //    public virtual DateTimeOffset CreateTime { get; set; }
    //    /// <summary>
    //    /// createBy
    //    /// </summary>
    //    [Column( "create_by")]
    //    public virtual string CreateBy { get; set; }
    //}

    //public class AuditEntity<KeyType> : CreateEntity<KeyType> ,IAuditEntity<KeyType>
    //{
    //    /// <summary>
    //    /// updateTime
    //    /// </summary>
    //    [Column( "update_time")]
    //    public virtual DateTimeOffset? UpdateTime { get; set; }
    //    /// <summary>
    //    /// updateBy
    //    /// </summary>
    //    [Column( "update_by")]
    //    public virtual string? UpdateBy { get; set; }
    //}


    public interface ICreateNameEntity 
    {
        public string CreatedByName { get; set; }
    }



    public interface IPageEntity<KeyType> : IEntity<KeyType>
    {
        int PageNum { get; set; }
        int PageSize { get; set; }
        int SkipCount { get;}
    }

    public class PageEntity<KeyType> : Entity<KeyType>, IPageEntity<KeyType>
    {

        
        public int PageNum { get; set; } = 1;

        
        public int PageSize { get; set; } = 20;

        
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
        
        public string? Keyword { get; set; }

        
        public List<OrderItem>? OrderList { get; set; }
        
        public string? DataScope { get; set; }

    }


    public class OrderItem
    {
        private string column;
        private bool asc;
    }
}
