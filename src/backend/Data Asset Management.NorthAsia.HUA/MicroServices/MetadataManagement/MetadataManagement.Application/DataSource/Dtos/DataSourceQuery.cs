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
    public class DataSourceQuery : PageEntity<long>
    {
        /// <summary>
        /// 备  注:资源类型:元数据类别
        /// 默认值:
        ///</summary>
        public string CatalogId { get; set; } = null!;

        /// <summary>
        /// 备  注:数据源编码
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
        /// 备  注:系统英文简称
        /// 默认值:
        ///</summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 备  注:业务系统类别/应用类型
        /// 默认值:
        ///</summary>
        public string SystemType { get; set; } = null!;

        /// <summary>
        /// 备  注:所属部门
        /// 默认值:
        ///</summary>
        public string? OwnerDept { get; set; }

        /// <summary>
        /// 备  注:SME NTID/ 负责人NTID/
        /// 默认值:
        ///</summary>
        public string? SmeNtid { get; set; }

        /// <summary>
        /// 备  注:SME 姓名
        /// 默认值:
        ///</summary>
        public string? SmeName { get; set; }

        /// <summary>
        /// 备  注:数据库类型/数据源类型
        /// 默认值:
        ///</summary>
        public string? DbType { get; set; }


        /// <summary>
        /// 备  注:基础信息,Json数据
        /// 默认值:
        ///</summary>
        public string? BaseInfo { get; set; }

        /// <summary>
        /// 备  注:技术信息,Json数据
        /// 默认值:
        ///</summary>
        public string? TechnologyInfo { get; set; }
    }
}