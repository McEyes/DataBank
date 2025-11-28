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
    public class DataTableDto
    {

        /// <summary>
        /// 备  注:唯一ID
        /// 默认值:
        ///</summary>
        public long Id { get; set; }

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
        /// 备  注:英文名称
        /// 默认值:
        ///</summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// 备  注:数据主题上级ID
        /// 默认值:
        ///</summary>
        public long? Parentid { get; set; }

        /// <summary>
        /// 备  注:备注说明
        /// 默认值:
        ///</summary
        public string? Remark { get; set; }

        /// <summary>
        /// 备  注:排序序号
        /// 默认值:
        ///</summary>
        public short? Sort { get; set; }

        /// <summary>
        /// 备  注:状态：1启用，0禁用
        /// 默认值:
        ///</summary>
        public short? Status { get; set; }

        /// <summary>
        /// 备  注:创建人名称
        /// 默认值:
        ///</summary>
        public string? CreateByName { get; set; }

        /// <summary>
        /// 备  注:更新人名称
        /// 默认值:
        ///</summary>
        public string? UpdateByName { get; set; }

    }
    
}