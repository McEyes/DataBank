using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using ITPortal.Core.Services;
namespace MetadataManagement.Core.Entitys
{
    /// <summary>
    /// 元数据类别
    ///</summary>
    public class DataCatalogQuery : PageEntity<long>
    {
        /// <summary>
        /// 备  注:编码
        /// 默认值:
        ///</summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// 备  注:中文名称
        /// 默认值:
        ///</summary>
        public string? Name { get; set; }

        /// <summary>
        /// 备  注:数据主题上级ID
        /// 默认值:
        ///</summary>
        public long? Parentid { get; set; }

        /// <summary>
        /// 备  注:状态：1启用，0禁用
        /// 默认值:
        ///</summary>
        public short? Status { get; set; }

    }
}