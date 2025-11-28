using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using ITPortal.Core.Services;
namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 表字段数据源信息
    ///</summary>
    [SugarTable("md_data_column")]
    public class DataColumnEntity: AuditEntity<long>
    {
        
     
        /// <summary>
        /// 备  注:唯一ID
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true,IsIdentity = true) ]
        public override long Id  { get; set;  } 
     
        /// <summary>
        /// 备  注:表编码
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="code" ) ]
        public string Code  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:中文名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="name" ) ]
        public string Name  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:字段类型
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="data_type" ) ]
        public string DataType  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:表Id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="table_id" ) ]
        public string TableId  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:数据源Id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="source_id" ) ]
        public string? SourceId  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源说明
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="from_desc" ) ]
        public string? FromDesc  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源字段
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="from_column_id" ) ]
        public long? FromColumnId  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源表
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="from_table_id" ) ]
        public long? FromTableId  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源数据库
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="from_database" ) ]
        public string? FromDatabase  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源系统/分类
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="from_catalog" ) ]
        public string? FromCatalog  { get; set;  } 
     
        /// <summary>
        /// 备  注:数据格式
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="data_format" ) ]
        public string? DataFormat  { get; set;  } 
     
        /// <summary>
        /// 备  注:中文描述
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="column_desc" ) ]
        public string? ColumnDesc  { get; set;  } 
     
        /// <summary>
        /// 备  注:英文描述
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="column_english_desc" ) ]
        public string? ColumnEnglishDesc  { get; set;  } 
     
        /// <summary>
        /// 备  注:是否为主键
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="is_key" ) ]
        public bool? IsKey  { get; set;  } 
     
        /// <summary>
        /// 备  注:显示顺序
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="sort" ) ]
        public short? Sort  { get; set;  } 
     
        /// <summary>
        /// 备  注:数据长度
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="data_length" ) ]
        public int? DataLength  { get; set;  } 
     
        /// <summary>
        /// 备  注:数据精度
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="data_precision" ) ]
        public int? DataPrecision  { get; set;  } 
     
        /// <summary>
        /// 备  注:数据小数位
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="data_scale" ) ]
        public int? DataScale  { get; set;  } 
     
        /// <summary>
        /// 备  注:度量单位
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="qa_units" ) ]
        public string? QaUnits  { get; set;  } 
     
        /// <summary>
        /// 备  注:取值范围
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="value_range" ) ]
        public string? ValueRange  { get; set;  } 
     
        /// <summary>
        /// 备  注:计算规则:QA规则列表
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="qa_rules" ) ]
        public string? QaRules  { get; set;  } 
     
        /// <summary>
        /// 备  注:备注说明
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="remark" ) ]
        public string? Remark  { get; set;  } 
     
        /// <summary>
        /// 备  注:安全级别
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="level_id" ) ]
        public string? LevelId  { get; set;  } 
     
        /// <summary>
        /// 备  注:技术信息,json数据
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="technology_info" ) ]
        public string? TechnologyInfo  { get; set;  } 
     
        /// <summary>
        /// 备  注:英文名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="english_name" ) ]
        public string? EnglishName  { get; set;  } 
     
        /// <summary>
        /// 备  注:物理字段名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="real_column_name" ) ]
        public string? RealColumnName  { get; set;  } 
     
        /// <summary>
        /// 备  注:主题域
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="topic" ) ]
        public string? Topic  { get; set;  } 
     
        /// <summary>
        /// 备  注:业务描述，业务含义
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="business_description" ) ]
        public string? BusinessDescription  { get; set;  } 
     

    }
    
}