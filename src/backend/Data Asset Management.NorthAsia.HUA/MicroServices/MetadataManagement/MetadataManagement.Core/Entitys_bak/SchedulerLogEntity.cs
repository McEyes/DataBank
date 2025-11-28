using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 采集日志记录
    /// </summary>
    public class SchedulerLogEntity
    {
        /// <summary>
        /// 采集任务英文名称
        /// </summary>
        public string JobCode { get; set; }
        /// <summary>
        /// 采集任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 数据源类型：mysql
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 数据源名称
        /// </summary>
        public long DataSource { get; set; }
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string DataSourceName { get; set; }


        /// <summary>
        /// 业务部门
        /// </summary>
        public string? BusinessDept { get; set; }
        /// <summary>
        /// SME NTID/ 负责人/业务负责人NTID
        /// </summary>
        public string? BusinessManager { get; set; }
        /// <summary>
        /// SME DisplayName/业务负责人名称
        /// </summary>
        public string? BusinessManagerName { get; set; }

        /// <summary>
        /// 采集类型
        /// </summary>
        public string JobType {  get; set; }
    }
}
