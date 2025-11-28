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
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core;
using SqlSugar;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [ElasticIndexName("DataTable", "DataAsset")]
    [SugarTable("metadata_table")]
    public class DataTableEntity : AuditEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public override string Id { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "source_id", Length = 255)]
        public string SourceId { get; set; }
        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string SourceName { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "table_name", Length = 255)]
        public string TableName { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "table_comment", Length = 255, IsNullable = true)]
        public string TableComment { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [SugarColumn(ColumnName = "alias")]
        public string Alias { get; set; }



        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public int Status { get; set; } = 1;


        ///// <summary>
        ///// 别名
        ///// </summary>
        //[SugarColumn(ColumnName = "create_org")]
        //public string CreateOrg { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [SugarColumn(ColumnName = "remark")]
        public string Remark { get; set; }


        ///// <summary>
        ///// 别名
        ///// </summary>
        //[SugarColumn(ColumnName = "db_type")]
        //public int? DbType { get; set; }


        ///// <summary>
        ///// 别名
        ///// </summary>
        //[SugarColumn(ColumnName = "is_sync")]
        //public int? IsSync { get; set; }

        ///// <summary>
        ///// 数据时间范围
        ///// </summary>
        //[SugarColumn(ColumnName = "db_schema")]
        //public string DbSchema { get; set; }

        /// <summary>
        /// 能否预览
        /// </summary>
        [SugarColumn(ColumnName = "reviewable")]
        public int? Reviewable { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "jsonsql_config", IsJson = true)]
        public JsonSqlConfig JsonSqlConfig { get; set; }

        /// <summary>
        /// 更新频率
        /// </summary>
        [SugarColumn(ColumnName = "update_frequency")]
        public string UpdateFrequency { get; set; }


        /// <summary>
        /// 更新方式  手动(默认)/自动 
        /// </summary>
        [SugarColumn(ColumnName = "update_method")]
        public string UpdateMethod { get; set; }
        /// <summary>
        /// 数据时间范围
        /// </summary>
        [SugarColumn(ColumnName = "data_time_range")]
        public string DataTimeRange { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string? OwnerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string? OwnerName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string? LevelId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string? LevelName { get; set; }


        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>
        [SugarColumn(ColumnName = "owner_depart", Length = 100)]

        public string OwnerDepart { get; set; }

        /// <summary>
        /// 质量分数
        /// </summary>
        [SugarColumn(ColumnName = "quality_score", Length = 100)]

        public int? QualityScore { get; set; }
        /// <summary>
        /// 数据分类：主数据和参考数据、业务记录数据、指标数据
        /// </summary>
        [SugarColumn(ColumnName = "data_category", Length = 100)]

        public string DataCategory { get; set; }

        /// <summary>
        /// 质量分数,上一次
        /// </summary>
        [SugarColumn(ColumnName = "last_score", Length = 100)]

        public int? LastScore { get; set; }
        /// <summary>
        /// 更新方法：全量，增量
        /// </summary>
        [SugarColumn(ColumnName = "update_category", Length = 50)]
        public string UpdateCategory { get; set; }



        [SugarColumn(IsIgnore = true)]
        public string CtlId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string CtlCode { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string CtlRemark { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string CtlName { get; set; }
        /// <summary>
        /// 访问次数，由服务更新
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int VisitsTimes { get; set; }
    }

    /// <summary>
    /// 自定义时间转换器
    /// </summary>
    public class CustomDateTimeConverter : JsonConverter<DateTimeOffset>
    {
        public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public override DateTimeOffset ReadJson(JsonReader reader, Type objectType, DateTimeOffset existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return DateTimeOffset.Parse(reader.Value.ToString());
        }
    }
}
