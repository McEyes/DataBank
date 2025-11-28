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
    public class DataColumnQuery : PageEntity<long>
    {

        /// <summary>
        /// 备  注:表编码
        /// 默认值:
        ///</summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// 备  注:中文名称
        /// 默认值:
        ///</summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 备  注:字段类型
        /// 默认值:
        ///</summary>
        public string DataType { get; set; } = null!;

        /// <summary>
        /// 备  注:表Id
        /// 默认值:
        ///</summary>
        public string TableId { get; set; } = null!;

        /// <summary>
        /// 备  注:数据源Id
        /// 默认值:
        ///</summary>
        public string? SourceId { get; set; }

        /// <summary>
        /// 备  注:来源说明
        /// 默认值:
        ///</summary>
        public string? FromDesc { get; set; }

        /// <summary>
        /// 备  注:来源字段
        /// 默认值:
        ///</summary>
        public long? FromColumnId { get; set; }

        /// <summary>
        /// 备  注:来源表
        /// 默认值:
        ///</summary>
        public long? FromTableId { get; set; }

        /// <summary>
        /// 备  注:来源数据库
        /// 默认值:
        ///</summary>
        public string? FromDatabase { get; set; }

        /// <summary>
        /// 备  注:来源系统/分类
        /// 默认值:
        ///</summary>
        public string? FromCatalog { get; set; }


        /// <summary>
        /// 备  注:中文描述
        /// 默认值:
        ///</summary>
        public string? ColumnDesc { get; set; }

        /// <summary>
        /// 备  注:英文描述
        /// 默认值:
        ///</summary>
        public string? ColumnEnglishDesc { get; set; }

        /// <summary>
        /// 备  注:是否为主键
        /// 默认值:
        ///</summary>
        public bool? IsKey { get; set; }



        /// <summary>
        /// 备  注:计算规则:QA规则列表
        /// 默认值:
        ///</summary>
        public string? QaRules { get; set; }


        /// <summary>
        /// 备  注:安全级别
        /// 默认值:
        ///</summary>
        public string? LevelId { get; set; }

        /// <summary>
        /// 备  注:技术信息,json数据
        /// 默认值:
        ///</summary>
        public string? TechnologyInfo { get; set; }

        /// <summary>
        /// 备  注:英文名称
        /// 默认值:
        ///</summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// 备  注:物理字段名称
        /// 默认值:
        ///</summary>
        public string? RealColumnName { get; set; }

        /// <summary>
        /// 备  注:主题域
        /// 默认值:
        ///</summary>
        public string? Topic { get; set; }

        /// <summary>
        /// 备  注:业务描述，业务含义
        /// 默认值:
        ///</summary>
        public string? BusinessDescription { get; set; }


    }
}