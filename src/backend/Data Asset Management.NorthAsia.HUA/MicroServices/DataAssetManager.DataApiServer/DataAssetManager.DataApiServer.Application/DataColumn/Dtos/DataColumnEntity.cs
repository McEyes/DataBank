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
using Microsoft.OpenApi.Extensions;
using ITPortal.Core.Extensions;

namespace DataAssetManager.DataApiServer.Application.DataColumn.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [ElasticIndexName("DataColumn", "DataAsset")]
    [SugarTable("metadata_column")]
    public class DataColumnEntity : Entity<string>
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
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "table_id", Length = 255)]
        public string TableId { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "column_name", Length = 255, IsNullable = true)]
        public string ColName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "column_comment", Length = 255, IsNullable = true)]
        public string ColComment { get; set; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        [SugarColumn(ColumnName = "column_key", Length = 255, IsNullable = true)]
        public string ColKey { get; set; }

        /// <summary>
        /// 是否容许为空
        /// </summary>
        [SugarColumn(ColumnName = "column_nullable", Length = 255, IsNullable = true)]
        public string Nullable { get; set; }
        /// <summary>
        /// 字段位置，序号
        /// </summary>
        [SugarColumn(ColumnName = "column_position", IsNullable = true)]
        public int? ColPosition { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        [SugarColumn(ColumnName = "data_type", Length = 255, IsNullable = true)]
        public string DataType { get; set; }
        /// <summary>
        /// 数据精度
        /// </summary>
        [SugarColumn(ColumnName = "data_length", Length = 255, IsNullable = true)]
        public string DataLength { get; set; }
        /// <summary>
        /// 数据精度
        /// </summary>
        [SugarColumn(ColumnName = "data_precision", Length = 255, IsNullable = true)]
        public string DataPrecision { get; set; }

        /// <summary>
        /// 数据小数位
        /// </summary>
        [SugarColumn(ColumnName = "data_scale", Length = 255, IsNullable = true)]
        public string DataScale { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        [SugarColumn(ColumnName = "data_default", Length = 255, IsNullable = true)]
        public string DataDefault { get; set; }
        /// <summary>
        /// 是否可排序
        /// </summary>
        [SugarColumn(ColumnName = "sortable", IsNullable = true)]
        public bool Sortable { get; set; }
        /// <summary>
        /// 是否为必要条件
        /// </summary>
        [SugarColumn(ColumnName = "required_as_condition", IsNullable = true)]
        public bool RequiredAsCondition { get; set; }
        /// <summary>
        /// 主数据类型：bay，workcell,equipment,function 等等
        /// </summary>
        [SugarColumn(ColumnName = "masterdata_type", Length = 255, IsNullable = true)]
        public string MasterdataType { get; set; }
        /// <summary>
        /// 是否标准化
        /// </summary>
        [SugarColumn(ColumnName = "standardized", IsNullable = true)]
        public bool Standardized { get; set; }

        /// <summary>
        /// 安全级别
        /// </summary>
        [SugarColumn(ColumnName = "security_level", Length = 50, IsNullable = true, DefaultValue = "2")]
        public string LevelId { get; set; } = "2";


        /// <summary>
        /// 列code
        /// </summary>
        [SugarColumn(ColumnName = "column_code", Length = 50, IsNullable = true)]
        public string ColumnCode { get; set; }


        /// <summary>
        /// indicator code
        /// 指标code
        /// </summary>
        [SugarColumn(ColumnName = "indicator_code", Length = 50, IsNullable = true)]
        public string IndicatorCode { get; set; }


        /// <summary>
        /// 质量分数
        /// </summary>
        [SugarColumn(ColumnName = "quality_score",IsNullable = true)]
        public decimal QualityScore { get; set; }
        /// <summary>
        /// 上一次质量分数
        /// </summary>
        [SugarColumn(ColumnName = "last_score", IsNullable = true)]
        public decimal LastScore { get; set; }


        /// <summary>
        /// 安全级别
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string LevelName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LevelId)) return "";
                return LevelId.GetEnum<SecurityLevel>().GetDescription();
            }
        }

        /// <summary>
        /// 表的级别有column的级别判断
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool IsPublicSecurityLevel
        {
            get
            {
                return LevelId == "1" || LevelId == "2" || LevelId == "" || LevelId == null;
            }
        }
    }

}
