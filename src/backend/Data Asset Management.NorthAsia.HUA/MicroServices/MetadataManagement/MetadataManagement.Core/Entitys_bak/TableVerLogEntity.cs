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
    /// 版本发布记录
    /// </summary>
    [Table("metadata_table_ver_log")]
    public class TableVerLogEntity : Entity<long>
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
        /// <summary>
        /// 之前版本
        /// </summary>
        public string OldVer { get; set; }
        /// <summary>
        /// 更新后版本
        /// </summary>
        public string NewVer { get; set; }
    }
}
