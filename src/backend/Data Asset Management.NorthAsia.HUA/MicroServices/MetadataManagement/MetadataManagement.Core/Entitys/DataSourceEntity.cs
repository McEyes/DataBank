using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using ITPortal.Core.Services;
namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 数据源
    ///</summary>
    [SugarTable("md_data_source")]
    public class DataSourceEntity: AuditEntity<long>
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
        /// 备  注:数据源编码
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="code" ) ]
        public string Code  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:中文名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="name" ) ]
        public string? Name  { get; set;  } 
     
        /// <summary>
        /// 备  注:英文名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="english_name" ) ]
        public string? EnglishName  { get; set;  } 
     
        /// <summary>
        /// 备  注:系统英文简称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="nick_name" ) ]
        public string? NickName  { get; set;  } 
     
        /// <summary>
        /// 备  注:业务系统类别/应用类型
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="system_type" ) ]
        public string SystemType  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:所属部门
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="owner_dept" ) ]
        public string? OwnerDept  { get; set;  } 
     
        /// <summary>
        /// 备  注:SME NTID/ 负责人NTID/
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="sme_ntid" ) ]
        public string? SmeNtid  { get; set;  } 
     
        /// <summary>
        /// 备  注:SME 姓名
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="sme_name" ) ]
        public string? SmeName  { get; set;  } 
     
        /// <summary>
        /// 备  注:数据库类型/数据源类型
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="db_type" ) ]
        public string? DbType  { get; set;  } 
     
        /// <summary>
        /// 备  注:备注说明
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="remark" ) ]
        public string? Remark  { get; set;  } 
     
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