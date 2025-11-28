using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using ITPortal.Core.Services;
namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 采集任务，采集器
    ///</summary>
    [SugarTable("md_scheduler_job")]
    public class SchedulerJobEntity: AuditEntity<long>
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
        public long CatalogId  { get; set;  } 
     
        /// <summary>
        /// 备  注:采集任务英文名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="code" ) ]
        public string Code  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:采集任务名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="name" ) ]
        public string Name  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:数据源id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="data_source_id" ) ]
        public long? DataSourceId  { get; set;  } 
     
        /// <summary>
        /// 备  注:数据库类型
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="dbtype" ) ]
        public string? Dbtype  { get; set;  } 
     
        /// <summary>
        /// 备  注:部门
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="dept" ) ]
        public string? Dept  { get; set;  } 
     
        /// <summary>
        /// 备  注:负责人NTID
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="manager" ) ]
        public string? Manager  { get; set;  } 
     
        /// <summary>
        /// 备  注:负责人名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="manager_name" ) ]
        public string? ManagerName  { get; set;  } 
     
        /// <summary>
        /// 备  注:采集范围:整库/自定义(具体选择哪些表采集)
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="pick_range" ) ]
        public string? PickRange  { get; set;  } 
     
        /// <summary>
        /// 备  注:采集周期配置Json格式
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="pick_cycle" ) ]
        public string? PickCycle  { get; set;  } 
     
        /// <summary>
        /// 备  注:采集corn表达式，采集周期最终转换成corn表达式
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="pick_corn" ) ]
        public string? PickCorn  { get; set;  } 
     
        /// <summary>
        /// 备  注:采集类型：自动手动
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="job_type" ) ]
        public string? JobType  { get; set;  } 
     
        /// <summary>
        /// 备  注:自动注册元数据:自动创建表，字段等信息
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="auto_reg_matadata" ) ]
        public bool? AutoRegMatadata  { get; set;  } 
     
        /// <summary>
        /// 备  注:采集范围为自定义时的tableid数组
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="pick_range_data" ) ]
        public string? PickRangeData  { get; set;  } 
    
   
     
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
     
        /// <summary>
        /// 备  注:备注说明
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="remark" ) ]
        public string? Remark  { get; set;  } 
     
        /// <summary>
        /// 备  注:状态：0草稿，1流程发布中，2已发布，3发布异常
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="status" ) ]
        public string? Status  { get; set;  } 
    

    

    }
    
}