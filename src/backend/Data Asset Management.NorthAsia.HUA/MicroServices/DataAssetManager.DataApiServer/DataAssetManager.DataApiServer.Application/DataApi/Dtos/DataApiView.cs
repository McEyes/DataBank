using ITPortal.Core;
using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Furion.JsonSerialization;
using StackExchange.Profiling.Internal;
using ITPortal.Core.LightElasticSearch;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    [SugarTable(TableName = "asset_data_api_view")]
    public class DataApiView : AuditEntity<string>, IStatusEntity
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "id", Length = 50)]
        public override string Id { get; set; }
        /// <summary>
        /// API名称
        /// </summary>
        [SugarColumn(ColumnName = "api_name", Length = 255)]
        public string ApiName { get; set; }

        /// <summary>
        /// API版本
        /// </summary>
        [SugarColumn(ColumnName = "api_version", Length = 10)]
        public string ApiVersion { get; set; }

        /// <summary>
        /// API路径
        /// </summary>
        [SugarColumn(ColumnName = "api_url", Length = 255)]
        public string ApiUrl { get; set; }
        /// <summary>
        /// API路径
        /// </summary>
        [JsonProperty]
        [SugarColumn(IsIgnore = true)]
        public string ApiServiceUrl
        {
            get { return $"/services/{ApiVersion}{ApiUrl}"; }
        }
        /// <summary>
        /// API路径
        /// </summary>
        [JsonProperty]
        [SugarColumn(IsIgnore = true)]
        public string ApiFullPath
        {
            get { return $"{ApiServiceUrl}"; }
        }

        /// <summary>
        /// 请求类型
        /// </summary>
        [SugarColumn(ColumnName = "req_method", Length = 10)]
        public string ReqMethod { get; set; }

        /// <summary>
        /// 返回格式
        /// </summary>
        [SugarColumn(ColumnName = "res_type", Length = 10)]
        public string ResType { get; set; }
        /// <summary>
        /// IP黑名单多个，隔开
        /// </summary>
        [SugarColumn(ColumnName = "deny", Length = 2000)]
        public string Deny { get; set; }
        /// <summary>
        /// 数据源id
        /// </summary>
        [SugarColumn(ColumnName = "source_id", Length = 50)]
        public string SourceId { get; set; }

        /// <summary>
        /// 限流配置
        /// </summary>
        [SugarColumn(ColumnName = "limit_json", IsJson = true)]
        public RateLimit RateLimit { get; set; }


        /// <summary>
        /// 执行配置
        /// </summary>
        [SugarColumn(ColumnName = "config_json", IsJson = true)]
        public ExecuteConfig ExecuteConfig { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [SugarColumn(ColumnName = "req_json", IsJson = true)]
        public List<ReqParam> ReqParams { get; set; }

        /// <summary>
        /// 返回字段
        /// </summary>
        [SugarColumn(ColumnName = "res_json", IsJson = true)]
        public List<ResParam> ResParams { get; set; }



        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "remark", Length = 1000)]
        public string Remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "table_id", Length = 1000)]
        public string TableId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "table_name", Length = 1000)]
        public string TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "table_comment", Length = 1000)]
        public string TableComment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "alias", Length = 1000)]
        public string TableAlias { get; set; }


        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "ctl_id", IsPrimaryKey = true)]
        public string CtlId { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "ctl_name")]
        public string? CtName { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "ctl_code")]
        public string? CtCode { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "ctl_remark")]
        public string? CtlRemark { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "parent_ctl_id")]
        public string? ParentCtlId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "level_id")]
        public string? LevelId { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "level_name")]
        public string? LevelName { get; set; }

        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [SugarColumn(ColumnName = "owner_depart", Length = 100)]

        public string OwnerDepart { get; set; }
        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [SugarColumn(ColumnName = "owner_id", Length = 100, IsIgnore = true)]

        public string OwnerId { get; set; }
        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [SugarColumn(ColumnName = "owner_name", Length = 100, IsIgnore = true)]

        public string OwnerName { get; set; }
        /// <summary>
        /// API标签
        /// </summary>
        [SugarColumn(ColumnName = "owner_name", Length = 100, IsIgnore = true)]

        public string Tags { get; set; }
    }
}
