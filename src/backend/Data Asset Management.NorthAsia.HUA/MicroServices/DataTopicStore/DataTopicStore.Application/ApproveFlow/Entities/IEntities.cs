using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.ApproveFlow.Entities
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
        //DateTime CreateTime { get; set; }
        ///// <summary>
        ///// createBy
        ///// </summary>
        //[SugarColumn(ColumnName = "create_by")]
        //string CreateBy { get; set; }

        ///// <summary>
        ///// updateTime
        ///// </summary>
        //[SugarColumn(ColumnName = "update_time")]
        //DateTime? UpdateTime { get; set; }
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
        [SugarColumn(ColumnName = "create_time")]
        DateTime CreateTime { get; set; }
        /// <summary>
        /// createBy
        /// </summary>
        [SugarColumn(ColumnName = "create_by")]
        string CreateBy { get; set; }


    }

    public interface IAuditEntity<KeyType> : ICreateEntity<KeyType>
    {

        /// <summary>
        /// updateTime
        /// </summary>
        [SugarColumn(ColumnName = "update_time")]
        DateTime? UpdateTime { get; set; }
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
        DateTime? DeleteTime { get; set; }
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

}
