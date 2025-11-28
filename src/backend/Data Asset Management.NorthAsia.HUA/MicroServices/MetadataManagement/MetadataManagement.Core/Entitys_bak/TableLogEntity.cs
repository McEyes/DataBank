using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 表操作记录:ddl/select/insert
    /// </summary>
    [Table("mt_table_log")]
    public class TableLogEntity : Entity<long>
    {
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 操作人NTID
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        public string OperatorName { get; set; }
        /// <summary>
        /// 操作动作：读/写/修改列/添加列/修改表名 等等
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// 执行sql
        /// </summary>
        public string ExecSql {  get; set; }
        /// <summary>
        /// 操作说明，描述
        /// </summary>
        public string Description { get; set; }
    }
}
