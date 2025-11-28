using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;

using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.ProxyApi.Dto;
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
    [Serializable]
    [ElasticIndexName("DataAuthApply", "DataAsset")]
    [SugarTable("metadata_auth_apply")]
    public class DataAuthApplyEntity : AuditEntity<Guid>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public override Guid Id { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "apply_form_id", IsIgnore = true)]
        public string FlowId
        {
            get { if (string.IsNullOrWhiteSpace(_flowId)) return Id.ToString(); else return _flowId; }
            set
            {
                _flowId = value;
                if (Guid.TryParse(_flowId, out Guid id)) Id = id;
            }
        }

        private string _flowId;
        /// <summary>
        /// 流程编号
        /// </summary>
        [SugarColumn(ColumnName = "apply_form_no")]
        public string? FlowNo { get; set; }


        /// <summary>
        /// 申请人ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id")]
        public string UserId { get; set; }
        /// <summary>
        /// 申请人姓名
        /// </summary>
        [SugarColumn(ColumnName = "user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// api的拥有者id，对应申请表单中的ownerid
        /// </summary>
        [SugarColumn(ColumnName = "sme_id")]
        public string? SmeId { get; set; }

        /// <summary>
        /// api的拥有者名称，对应申请表单中的ownername
        /// </summary>
        [SugarColumn(ColumnName = "sme_name")]
        public string? SmeName { get; set; }

        /// <summary>
        /// api的拥有者所属部门，对应申请表单中的ownerdept
        /// </summary>
        [SugarColumn(ColumnName = "sme_dept")]
        public string SmeDept { get; set; }


        /// <summary>
        /// 状态:-1发起，-2发起失败，0通过，1拒绝,-3取消
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "remark")]
        public string? Remark { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        [SugarColumn(ColumnName = "reason")]
        public string? Reason { get; set; }

        /// <summary>
        /// 当前审批节点
        /// </summary>
        [SugarColumn(ColumnName = "apply_node",IsJson =true)]
        public List<ApplyFlowCommonNodeInput>? ApplyNode { get; set; }

        /// <summary>
        /// 当前审批人
        /// </summary>
        [SugarColumn(ColumnName = "approver")]
        public string? Approver { get; set; }

        /// <summary>
        /// 流程完成时间
        /// </summary>
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [SugarColumn(ColumnName = "completion_time", IsOnlyIgnoreInsert = true)]
        public DateTimeOffset? CompletionTime { get; set; }

        /// <summary>
        /// 流程完成时间
        /// </summary>
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [SugarColumn(ColumnName = "audit_time", IsOnlyIgnoreInsert = true)]
        public DateTimeOffset? AuditTime { get; set; }

        /// <summary>
        /// 拥有人
        /// </summary>
        [SugarColumn(ColumnName = "api_token")]
        public string? Token { get; set; }

        ///// <summary>
        ///// 拥有人
        ///// </summary>
        //[SugarColumn(ColumnName = "owner")]
        //public string? Owner { get; set; }
        ///// <summary>
        ///// 拥有人姓名
        ///// </summary>
        //[SugarColumn(ColumnName = "owner_name")]
        //public string? OwnerName { get; set; }

        ///// <summary>
        ///// 拥有人部门
        ///// </summary>
        //[SugarColumn(ColumnName = "owner_dept")]
        //public string? OwnerDept { get; set; }

        /// <summary>
        /// 所有表Id,json array
        /// </summary>
        [SugarColumn(ColumnName = "table_id")]
        public string TableId { get; set; } 
        [SugarColumn(ColumnName = "table_name")]
        public string? TableName { get; set; }
        [SugarColumn(ColumnName = "table_code")]
        public string? TableCode { get; set; }
        [SugarColumn(ColumnName = "table_comment")]
        public string? TableComment { get; set; }
        /// <summary>
        /// 所有表Id,json array
        /// </summary>
        [SugarColumn(ColumnName = "table_columns", IsJson = true)]
        public List<DataColumnInfo> ColumnList { get; set; }
        /// <summary>
        /// DataCatalog id字符串，用逗号分隔
        /// </summary>
        [SugarColumn(ColumnName = "ctl_id")]
        public string? CtlId { get; set; }
        /// <summary>
        /// DataCatalog name字符串，用逗号分隔
        /// </summary>
        [SugarColumn(ColumnName = "ctl_name")]
        public string? CtlName { get; set; }
        /// <summary>
        /// DataCatalog name字符串，用逗号分隔
        /// </summary>
        [SugarColumn(ColumnName = "ctl_code")]
        public string? CtlCode { get; set; }
        /// <summary>
        /// DataCatalog remark字符串，用逗号分隔
        /// </summary>
        [SugarColumn(ColumnName = "ctl_remark")]
        public string? CtlRemark { get; set; }
        /// <summary>
        /// 申请类型：1个人，2app
        /// </summary>
        [SugarColumn(ColumnName = "apply_type")]
        public int ApplyType { get; set; }
        /// <summary>
        /// app id
        /// </summary>
        [SugarColumn(ColumnName = "app_id")]
        public string? AppId { get; set; }
        /// <summary>
        /// app name
        /// </summary>
        [SugarColumn(ColumnName = "app_name")]
        public string? AppName { get; set; }
        /// <summary>
        /// app owner name
        /// </summary>
        [SugarColumn(ColumnName = "app_owner")]
        public string? AppOwner { get; set; }

        /// <summary>
        /// 涉密级别
        /// </summary>
        [SugarColumn(ColumnName = "level_id")]
        public string? LevelId { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool IsPublicSecurityLevel
        {
            get
            {
                return LevelId == "1" || LevelId == "2";
            }
        }

        [SugarColumn(ColumnName = "needsup")]
        public int? NeedSup { get; set; }
    }

}
