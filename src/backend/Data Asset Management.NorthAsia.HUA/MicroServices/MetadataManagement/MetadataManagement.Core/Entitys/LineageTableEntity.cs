using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using ITPortal.Core.Services;
namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 表血缘关系表
    ///</summary>
    [SugarTable("md_lineage_table")]
    public class LineageTableEntity: AuditEntity<long>
    {
        
     
        /// <summary>
        /// 备  注:唯一ID,溯源表id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true,IsIdentity = true) ]
        public override long Id  { get; set;  } 
     
        /// <summary>
        /// 备  注:溯源字段id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="table_id" ) ]
        public long TableId  { get; set;  } 
     
        /// <summary>
        /// 备  注:溯源父级id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="parent_id" ) ]
        public long? ParentId  { get; set;  } 
     
        /// <summary>
        /// 备  注:数据源
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="source_id" ) ]
        public long? SourceId  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源表,上游表id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="up_table_id" ) ]
        public long? UpTableId  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源表,上游表，显示名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="up_table_name" ) ]
        public string? UpTableName  { get; set;  } 
     
        /// <summary>
        /// 备  注:上游表显示顺序
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="up_table_sort" ) ]
        public int? UpTableSort  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源完整sql,或者表达式
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="up_from_sql" ) ]
        public string? UpFromSql  { get; set;  } 
     
        /// <summary>
        /// 备  注:血缘id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="lineage_id" ) ]
        public long LineageId  { get; set;  } 
     
        /// <summary>
        /// 备  注:血缘关系名称， namespace对应table的catalog
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="name" ) ]
        public string? Name  { get; set;  } 
     
        /// <summary>
        /// 备  注:血缘类型：DB_TABLE
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="lineage_type" ) ]
        public string? LineageType  { get; set;  } 
     
        /// <summary>
        /// 备  注:血缘tag
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="tags" ) ]
        public string? Tags  { get; set;  } 
     
        /// <summary>
        /// 备  注:血缘描述说明
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="description" ) ]
        public string? Description  { get; set;  } 
    

    

    }
    
}