using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using ITPortal.Core.Services;
namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 字段血缘关系表
    ///</summary>
    [SugarTable("md_lineage_column")]
    public class LineageColumnEntity: AuditEntity<long>
    {
        
     
        /// <summary>
        /// 备  注:唯一ID
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true,IsIdentity = true) ]
        public override long Id  { get; set;  } 
     
        /// <summary>
        /// 备  注:溯源字段id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="column_id" ) ]
        public long ColumnId  { get; set;  } 
     
        /// <summary>
        /// 备  注:上游字段id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="up_column_id" ) ]
        public long? UpColumnId  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源表
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="up_table_id" ) ]
        public long? UpTableId  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源数据库
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="up_database" ) ]
        public string? UpDatabase  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源系统/分类
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="up_catalog" ) ]
        public string? UpCatalog  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源类型：1单一字段来源，2，多字段简单拼接，3多字段计算 等等
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="up_type" ) ]
        public string? UpType  { get; set;  } 
     
        /// <summary>
        /// 备  注:来源完整sql
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="up_from_sql" ) ]
        public string? UpFromSql  { get; set;  } 
     
        /// <summary>
        /// 备  注:溯源父级id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="parent_id" ) ]
        public long? ParentId  { get; set;  } 
     
        /// <summary>
        /// 备  注:血缘tableid
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="lineage_table_id" ) ]
        public long LineageTableId  { get; set;  } 
     
        /// <summary>
        /// 备  注:血缘id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="lineage_id" ) ]
        public long LineageId  { get; set;  } 
     
        /// <summary>
        /// 备  注:溯源表id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="table_id" ) ]
        public long TableId  { get; set;  } 
    

    

    }
    
}