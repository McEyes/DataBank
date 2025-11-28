using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using ITPortal.Core.Services;
namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 数据血缘关系
    ///</summary>
    [SugarTable("md_lineage")]
    public class LineageEntity: AuditEntity<long>
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
        /// 备  注:血缘编码
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="code" ) ]
        public string Code  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:中文名称，处理节点名称
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
        /// 备  注:主题域
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="topic" ) ]
        public string? Topic  { get; set;  } 
     
        /// <summary>
        /// 备  注:业务描述：处理节点描述说明
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="business_description" ) ]
        public string? BusinessDescription  { get; set;  } 
     
        /// <summary>
        /// 备  注:血缘类型（节点类型）：table/etl/sql/ table类型才能定位溯源，其他类型的只是血缘关系中间过程
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="lineage_type" ) ]
        public string LineageType  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:溯源表id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="table_id" ) ]
        public long? TableId  { get; set;  } 
     
     
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