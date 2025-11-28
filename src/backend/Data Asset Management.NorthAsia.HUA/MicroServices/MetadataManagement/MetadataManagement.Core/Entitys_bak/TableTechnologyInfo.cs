using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataManagement.Core.Entitys
{
    public class TableTechnologyInfo
    {
        /// <summary>
        /// 所属分层
        /// </summary>
        public string BelongLayer { get; set; }

        /// <summary>
        /// 数据源类型
        /// </summary>
        public string? DbType { get; set; }
        /// <summary>
        /// 数据库名/数据源
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// 所属集群
        /// </summary>
        public string BelongCluster { get; set; }
        /// <summary>
        /// 存储位置
        /// </summary>
        public string? StorageLocation { get; set; }
        /// <summary>
        /// 存储大小
        /// </summary>
        public long StorageSize { get; set; }

        /// <summary>
        /// <summary>
        /// 分区信息
        /// </summary>
        public long LayerType { get; set; }


        /// 表类型：内部表/外部表
        /// </summary>
        public string TableType { get; set; }

        /// <summary>
        /// 表类型：内部表/外部表
        /// </summary>
        public string CreateTableSql { get; set; }

        /// <summary>
        /// 表索引
        /// </summary>
        public string TableIndex { get; set; }

    }
}
