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
    /// 元数据类别
    /// </summary>
    [Serializable]
    //[ElasticIndexName("DataCatalog", "DataAsset")]
    [Table("md_catalog")]
    public class DataCatalogEntity : AuditEntity<long>, IAuditNameEntity
    {
        /// <summary>
        /// 目录编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 数据主题中文名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 数据主题英文名称
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// 数据主题上级ID
        /// </summary>
        public string? ParentId { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 状态：1启用，0禁用
        /// </summary>
        public int? Status { get; set; }
        public string CreatedByName { get; set; }
        public string UpdateByName { get; set; }

    }
}
