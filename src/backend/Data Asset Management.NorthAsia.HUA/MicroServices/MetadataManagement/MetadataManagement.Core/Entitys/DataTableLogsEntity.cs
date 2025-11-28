using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using ITPortal.Core.Services;
namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 表的操作记录：新增，修改，查询等
    ///</summary>
    [SugarTable("md_data_table_logs")]
    public class DataTableLogsEntity: AuditEntity<long>
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
        [SugarColumn(ColumnName="table_id" ) ]
        public long TableId  { get; set;  } 
     
        /// <summary>
        /// 备  注:创建时间，操作时间
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="create_date" ) ]
        public DateTime CreateDate  { get; set;  } 
     
        /// <summary>
        /// 备  注:操作人NTID
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="operator" ) ]
        public string Operator  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:操作人名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="operator_name" ) ]
        public string OperatorName  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:操作动作：读/写/修改列/添加列/修改表名 等等
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="operation" ) ]
        public string? Operation  { get; set;  } 
     
        /// <summary>
        /// 备  注:执行sql
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="execsql" ) ]
        public string? Execsql  { get; set;  } 
     
        /// <summary>
        /// 备  注:操作说明
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="description" ) ]
        public string? Description  { get; set;  } 
     
        /// <summary>
        /// 备  注:操作结果
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="status" ) ]
        public string? Status  { get; set;  } 
    

    

    }
    
}