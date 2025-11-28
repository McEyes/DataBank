using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using ITPortal.Core.Services;
namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 采集任务执行日志
    ///</summary>
    [SugarTable("md_scheduler_logs")]
    public class SchedulerLogsEntity: AuditEntity<long>
    {
        
     
        /// <summary>
        /// 备  注:唯一ID
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true,IsIdentity = true) ]
        public override long Id  { get; set;  } 
     
        /// <summary>
        /// 备  注:采集任务id
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="job_id" ) ]
        public long JobId  { get; set;  } 
     
        /// <summary>
        /// 备  注:采集任务运行编码
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="code" ) ]
        public string Code  { get; set;  } = null!;
    
     
        /// <summary>
        /// 备  注:创建人名称
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="create_by_name" ) ]
        public string? CreateByName  { get; set;  } 
     
        /// <summary>
        /// 备  注:状态：0待执行，1正在执行，2执行完成，3执行异常
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="status" ) ]
        public string Status  { get; set;  } = null!;
     
        /// <summary>
        /// 备  注:异常信息
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="error_msg" ) ]
        public string? ErrorMsg  { get; set;  } 
     
        /// <summary>
        /// 备  注:运行日志
        /// 默认值:
        ///</summary>
        [SugarColumn(ColumnName="run_msg" ) ]
        public string? RunMsg  { get; set;  } 
    

    

    }
    
}