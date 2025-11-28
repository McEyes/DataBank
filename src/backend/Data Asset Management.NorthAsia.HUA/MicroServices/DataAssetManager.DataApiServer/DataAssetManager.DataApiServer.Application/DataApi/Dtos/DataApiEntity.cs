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
    [ElasticIndexName("DataApi", "DataAsset")]
    [SugarTable(TableName = "asset_data_api")]
    public class DataApiEntity : AuditEntity<string>, IStatusEntity
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "id",Length =50)]
        public override string Id { get; set; }

        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }
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
        /// 数据源id
        /// </summary>
        [SugarColumn(ColumnName = "source_id", Length = 50)]
        public string SourceId
        {
            get
            {
                if (_sourceId.IsNullOrWhiteSpace() && ExecuteConfig != null) return ExecuteConfig.sourceId;
                else return _sourceId;
            }
            set { _sourceId = value; }
        }
        private string _sourceId;

        /// <summary>
        /// IP黑名单多个，隔开
        /// </summary>
        [SugarColumn(ColumnName = "deny", Length = 2000)]
        public string Deny { get; set; }

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
        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [SugarColumn(ColumnName = "owner_depart", Length = 100)]

        public string OwnerDepart { get; set; }
    }
}
