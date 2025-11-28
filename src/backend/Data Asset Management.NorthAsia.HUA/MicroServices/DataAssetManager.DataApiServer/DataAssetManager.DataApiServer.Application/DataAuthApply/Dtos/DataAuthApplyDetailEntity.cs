using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.Services;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [ElasticIndexName("DataAuthApplyDetail", "DataAsset")]
    [Serializable]
    [SugarTable("metadata_auth_apply_detail")]
    public class DataAuthApplyDetailEntity : Entity<Guid>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public override Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 流程id
        /// </summary>
        [SugarColumn(ColumnName = "flow_id")]
        public Guid? FlowId { get; set; }

        /// <summary>
        /// 流程编号
        /// </summary>
        [SugarColumn(ColumnName = "flow_no")]
        public string? FlowNo { get; set; }


        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "object_id")]
        public string? ObjectId { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "object_name")]
        public string? ObjectName { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "table_columns", IsJson = true)]
        public List<DataColumn.Dtos.DataColumnEntity>? TableColumns { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "owner_id")]
        public string? OwnerId { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "owner_name")]
        public string? OwnerName { get; set; }
        /// <summary>
        /// 拥有人部门
        /// </summary>
        [SugarColumn(ColumnName = "owner_dept")]
        public string? OwnerDept { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "ctl_id")]
        public string? CtlId { get; set; }
        /// <summary>
        /// DataCatalog id
        /// </summary>
        [SugarColumn(ColumnName = "ctl_name")]
        public string? CtlName { get; set; }
        /// <summary>
        /// DataCatalog id
        /// </summary>
        [SugarColumn(ColumnName = "ctl_remark")]
        public string? CtlRemark { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "object_type")]
        public string? ObjectType { get; set; }
        /// <summary>
        /// 涉密级别
        /// </summary>
        [SugarColumn(ColumnName = "security_level")]
        public string? LevelId { get; set; }
        /// <summary>
        /// 涉密级别
        /// </summary>
        [SugarColumn(ColumnName = "all_columns")]
        public bool? AllColumns { get; set; }
        //public string UserId { get; internal set; }
    }

}
