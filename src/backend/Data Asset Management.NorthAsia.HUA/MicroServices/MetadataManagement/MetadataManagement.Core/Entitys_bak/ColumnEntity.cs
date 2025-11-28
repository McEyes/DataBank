
using ITPortal.Core.Services;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAssetManager.DataApiServer.Application.DataColumn.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    //[ElasticIndexName("DataColumn", "DataAsset")]
    [Table("metadata_column")]
    public class ColumnEntity : Entity<long>
    {
        /// <summary>
        /// 所属数据源
        /// </summary>
        //[SugarColumn(ColumnName = "source_id", Length = 255)]
        public string SourceId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        //[SugarColumn(ColumnName = "table_id", Length = 255)]
        public string TableId { get; set; }
        /// <summary>
        /// 字段字段编码
        /// </summary>
        //[SugarColumn(ColumnName = "column_name", Length = 255, IsNullable = true)]
        public string ColCode { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        //[SugarColumn(ColumnName = "column_name", Length = 255, IsNullable = true)]
        public string ColName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        //[SugarColumn(ColumnName = "data_type", Length = 255, IsNullable = true)]
        public string DataType { get; set; }
        /// <summary>
        /// 来源说明
        /// </summary>
        //[SugarColumn(ColumnName = "column_comment", Length = 255, IsNullable = true)]
        public string FromDesc { get; set; }//Description
        /// <summary>
        /// 来源字段
        /// </summary>
        //[SugarColumn(ColumnName = "column_comment", Length = 255, IsNullable = true)]
        public string FromColumn { get; set; }//Description
        /// <summary>
        /// 来源表
        /// </summary>
        //[SugarColumn(ColumnName = "column_comment", Length = 255, IsNullable = true)]
        public string FromTable { get; set; }//Description
        /// <summary>
        /// 来源数据库
        /// </summary>
        //[SugarColumn(ColumnName = "column_comment", Length = 255, IsNullable = true)]
        public string FromDb { get; set; }//Description
        /// <summary>
        /// 来源系统/分类/数据库
        /// </summary>
        //[SugarColumn(ColumnName = "column_comment", Length = 255, IsNullable = true)]
        public string FromCatalog { get; set; }//Description

        /// <summary>
        /// 数据格式
        /// </summary>
        //[SugarColumn(ColumnName = "column_comment", Length = 255, IsNullable = true)]
        public string ColFormat { get; set; }

        /// <summary>
        /// 中文描述
        /// </summary>
        //[SugarColumn(ColumnName = "column_comment", Length = 255, IsNullable = true)]
        public string ColDesc { get; set; }
        /// <summary>
        /// 英文描述
        /// </summary>
        //[SugarColumn(ColumnName = "column_comment", Length = 255, IsNullable = true)]
        public string ColEnglishDesc { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        //[SugarColumn(ColumnName = "column_key", Length = 255, IsNullable = true)]
        public string ColKey { get; set; }

        /// <summary>
        /// 是否容许为空
        /// </summary>
        //[SugarColumn(ColumnName = "column_nullable", Length = 255, IsNullable = true)]
        public string Nullable { get; set; }
        /// <summary>
        /// 字段位置，序号
        /// </summary>
        //[SugarColumn(ColumnName = "column_position", IsNullable = true)]
        public int? ColPosition { get; set; }
        /// <summary>
        /// 数据精度
        /// </summary>
        //[SugarColumn(ColumnName = "data_length", Length = 255, IsNullable = true)]
        public string DataLength { get; set; }
        /// <summary>
        /// 数据精度
        /// </summary>
        //[SugarColumn(ColumnName = "data_precision", Length = 255, IsNullable = true)]
        public string DataPrecision { get; set; }

        /// <summary>
        /// 数据小数位
        /// </summary>
        //[SugarColumn(ColumnName = "data_scale", Length = 255, IsNullable = true)]
        public string DataScale { get; set; }
        /// <summary>
        /// 度量单位
        /// </summary>
        //[SugarColumn(ColumnName = "data_default", Length = 255, IsNullable = true)]
        public string Units { get; set; }
        /// <summary>
        /// 取值范围
        /// </summary>
        //[SugarColumn(ColumnName = "data_default", Length = 255, IsNullable = true)]
        public string ValueRange { get; set; }

        /// <summary>
        /// 计算规则:QA规则列表
        /// </summary>
        //[SugarColumn(ColumnName = "data_default", Length = 255, IsNullable = true)]
        public string QARules { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        ///// <summary>
        ///// 是否为必要条件
        ///// </summary>
        ////[SugarColumn(ColumnName = "required_as_condition", IsNullable = true)]
        //public bool RequiredAsCondition { get; set; }
        ///// <summary>
        ///// 主数据类型：bay，workcell,equipment,function 等等
        ///// </summary>
        ////[SugarColumn(ColumnName = "masterdata_type", Length = 255, IsNullable = true)]
        //public string MasterdataType { get; set; }
        ///// <summary>
        ///// 是否标准化
        ///// </summary>
        ////[SugarColumn(ColumnName = "standardized", IsNullable = true)]
        //public bool Standardized { get; set; }

        /// <summary>
        /// 安全级别
        /// </summary>
      //  [SugarColumn(ColumnName = "security_level", Length = 50, IsNullable = true, DefaultValue = "2")]
        public string LevelId { get; set; } = "2";

        //  /// <summary>
        //  /// 安全级别
        //  /// </summary>
        ////  [SugarColumn(IsIgnore = true)]
        //  public string LevelName
        //  {
        //      get
        //      {
        //          if (string.IsNullOrWhiteSpace(LevelId)) return "";
        //          return LevelId.GetEnum<SecurityLevel>().GetDescription();
        //      }
        //  }

        //  /// <summary>
        //  /// 表的级别有column的级别判断
        //  /// </summary>
        ////  [SugarColumn(IsIgnore = true)]
        //  public bool IsPublicSecurityLevel
        //  {
        //      get
        //      {
        //          return LevelId == "1" || LevelId == "2" || LevelId == "" || LevelId == null;
        //      }
        //  }
        /// <summary>
        /// 技术信息
        /// </summary>
        [Column("technology_info")]
        public string TechnologyJson { get; set; }
    }

}
