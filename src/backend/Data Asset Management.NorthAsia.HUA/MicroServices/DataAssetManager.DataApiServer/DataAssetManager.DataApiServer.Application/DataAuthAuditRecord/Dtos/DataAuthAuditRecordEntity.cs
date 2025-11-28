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
    [Serializable]
    [ElasticIndexName("DataAuthAuditRecord", "DataAsset")]
    [SugarTable("metadata_auth_audit_record")]
    public class DataAuthAuditRecordEntity : AuditEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "apply_form_id", IsPrimaryKey = true)]
        public override string Id { get; set; }


        [SugarColumn(IsIgnore = true)]
        public string ApplyFormId { get { return Id; } set { Id = value; } }

        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "apply_form_no")]
        public string? ApplyFormNo { get; set; }

        /// <summary>
        /// 表信息清单
        /// </summary>
        [SugarColumn(ColumnName = "table_id",IsJson =true)]
        public AuthTableInfo[] TableId { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "table_name")]
        public string? TableName { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "user_id")]
        public string? UserId { get; set; }

        ///// <summary>
        ///// 表操作规则配置
        ///// </summary>
        //[SugarColumn(ColumnName = "create_by")]
        //public string? create_by { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "sme_id")]
        public string? SmeId { get; set; }
        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "sme_name")]
        public string? SmeName { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "remark")]
        public string? Remark { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "apply_node")]
        public string? ApplyNode { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "table_code")]
        public string? TableCode { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "api_token")]
        public string? ApiToken { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "reason")]
        public string? Reason { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [SugarColumn(ColumnName = "audit_time", IsOnlyIgnoreInsert = true)]
        public DateTimeOffset? AuditTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [SugarColumn(ColumnName = "require_time", IsOnlyIgnoreInsert = true)]
        public DateTimeOffset? RequireTime { get; set; }

        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //[JsonConverter(typeof(CustomDateTimeConverter))]
        //[SugarColumn(ColumnName = "create_time", IsOnlyIgnoreInsert = true)]
        //public DateTimeOffset CreateTime { get; set; }

        ///// <summary>
        ///// 创建人
        ///// </summary>
        //[SugarColumn(ColumnName = "create_by", IsOnlyIgnoreInsert = true)]
        //public string CreateBy { get; set; }

        ///// <summary>
        ///// 更新时间
        ///// </summary>
        //[JsonConverter(typeof(CustomDateTimeConverter))]
        //[SugarColumn(ColumnName = "update_time", IsOnlyIgnoreInsert = true)]
        //public DateTimeOffset UpdateTime { get; set; }

        ///// <summary>
        ///// 更新人
        ///// </summary>
        //[SugarColumn(ColumnName = "update_by", IsOnlyIgnoreInsert = true)]
        //public string UpdateBy { get; set; }
    }

}
