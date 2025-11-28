using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataManagement.Core.Entitys
{
    public class SchedulerEntity
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
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 资源类型
        /// </summary>
        public long DataCatalogId { get; set; }

        /// <summary>
        /// 数据源名称
        /// </summary>
        public long DataSourceId { get; set; }


        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }


        /// <summary>
        /// 采集范围:整库/自定义(具体选择哪些表采集)
        /// </summary>
        public string PickRange { get; set; }

        /// <summary>
        /// 采集周期配置Json格式
        /// </summary>
        public string PickCycle { get; set; }

        /// <summary>
        /// 采集corn表达式，采集周期最终转换成corn表达式
        /// </summary>
        public string PickCorn { get; set; }

        /// <summary>
        /// 采集类型：自动手动
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// 自动注册元数据:自动创建表，字段等信息
        /// </summary>
        public bool AutoRegMatadata { get; set; }
    }
}
