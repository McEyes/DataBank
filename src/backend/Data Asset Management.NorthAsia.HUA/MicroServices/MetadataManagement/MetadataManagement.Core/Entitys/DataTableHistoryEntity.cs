using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using ITPortal.Core.Services;
namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 表数据的版本记录
    ///</summary>
    [SugarTable("md_data_table_history")]
    public class DataTableHistoryEntity: AuditEntity<long>
    {
        
     
        /// <summary>
        /// 备  注:唯一ID
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true,IsIdentity = true) ]
        public override long Id  { get; set;  } 
     
        /// <summary>
        /// 备  注:资源类型:元数据类别
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="catalog_id" ) ]
        public string CatalogId  { get; set;  } = null!;
     
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
        /// 备  注:英文名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="english_name" ) ]
        public string? EnglishName  { get; set;  } 
     
        /// <summary>
        /// 备  注:物理表名
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="real_table_name" ) ]
        public string? RealTableName  { get; set;  } 
     
        /// <summary>
        /// 备  注:业务系统类别/应用类型
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="system_type" ) ]
        public string? SystemType  { get; set;  } 
     
        /// <summary>
        /// 备  注:业务部门
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="business_dept" ) ]
        public string? BusinessDept  { get; set;  } 
     
        /// <summary>
        /// 备  注:业务负责人NTID
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="business_manager" ) ]
        public string? BusinessManager  { get; set;  } 
     
        /// <summary>
        /// 备  注:业务负责人名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="business_manager_name" ) ]
        public string? BusinessManagerName  { get; set;  } 
     
        /// <summary>
        /// 备  注:安全级别
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="level_id" ) ]
        public string? LevelId  { get; set;  } 
     
        /// <summary>
        /// 备  注:主题域
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="topic" ) ]
        public string? Topic  { get; set;  } 
     
        /// <summary>
        /// 备  注:业务描述
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="business_description" ) ]
        public string? BusinessDescription  { get; set;  } 
     
        /// <summary>
        /// 备  注:版本：修改表结构的时候更新版本
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="ver" ) ]
        public int? Ver  { get; set;  } 
     
        /// <summary>
        /// 备  注:备注说明
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="remark" ) ]
        public string? Remark  { get; set;  } 
     
        /// <summary>
        /// 备  注:表行数
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="total_rows" ) ]
        public int? TotalRows  { get; set;  } 
     
        /// <summary>
        /// 备  注:表空间
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="total_size" ) ]
        public int? TotalSize  { get; set;  } 
     
        /// <summary>
        /// 备  注:基础信息,Json数据
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="base_info" ) ]
        public string? BaseInfo  { get; set;  } 
     
        /// <summary>
        /// 备  注:技术信息,Json数据
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="technology_info" ) ]
        public string? TechnologyInfo  { get; set;  } 
     
        /// <summary>
        /// 备  注:业务信息,Json数据
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="business_info" ) ]
        public string? BusinessInfo  { get; set;  } 
     
     
        /// <summary>
        /// 备  注:创建人名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="create_by_name" ) ]
        public string? CreateByName  { get; set;  } 
     
        /// <summary>
        /// 备  注:更新人名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="update_by_name" ) ]
        public string? UpdateByName  { get; set;  } 
    

    

    }
    
}