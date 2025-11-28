using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataManagement.Core.Entitys
{
    [Table("md_schema_table")]
    public class TableEntity: Entity<long>
    {
        /// <summary>
        /// 所属资产目录:元数据类别
        /// </summary>
        public long DataCatalogId { get; set; }

        /// <summary>
        /// 表编码:表编码code生成规则：
        /// 1. 业务系统/数据治理/ETL工具：CN02-[系统所属部门缩写]-[系统缩写]-[tablename前3个字母]-5位流水号
        /// - 流水号：00001-99999
        /// - 系统缩写 2-4位
        /// - databasename 3 位
        /// - 数据治理工具写 DG
        /// - ETL工具写DT
        /// 2.数据湖表编码:CN02-[主题域编码]-[层级（如ODS）]-[库名除去层级后的名称的前3个字母]-[表名按照_ split, 获取倒数第二个单词4个字母，不足，左补0）]-00001
        /// - 流水号范围：00001-99999
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 来源系统
        /// </summary>
        public long SourceCatalogId { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string? EnglishName { get; set; }

        ///// <summary>
        ///// 表英文简称
        ///// </summary>
        //public string? NickName { get; set; }
        /// <summary>
        /// 表物理名称
        /// </summary>
        public string? RealTableName { get; set; }

        /// <summary>
        /// 业务系统类别/应用类型
        /// </summary>
        public string? SystemType { get; set; }
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
        /// 安全级别
        /// </summary>
        //[SugarColumn(ColumnName = "security_level", Length = 50, IsNullable = true, DefaultValue = "2")]
        public string LevelId { get; set; } = "2";

        /// <summary>
        /// 安全级别
        /// </summary>
        //[SugarColumn(IsIgnore = true)]
        //public string LevelName
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(LevelId)) return "";
        //        return LevelId.GetEnum<SecurityLevel>().GetDescription();
        //    }
        //}
        ///// <summary>
        ///// 所属资产目录
        ///// </summary>
        //public string DataAssetCtlId { get; set; }

        /// <summary>
        /// 主题域
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 业务描述
        /// </summary>
        public string BusinessDescription { get; set; }

        /// <summary>
        /// 版本：修改表结构的时候更新版本
        /// </summary>
        public int Ver { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }



        /// <summary>
        /// 表行数
        /// </summary>
        public long TotalRows { get; set; }

        /// <summary>
        /// 表空间
        /// </summary>
        public long TableSize { get; set; }

        /// <summary>
        /// 基础信息
        /// </summary>
        [Column("base_info")]
        public string BaseJson { get; set; }

        /// <summary>
        /// 业务信息
        /// </summary>
        [Column("business_info")]
        public string businessJson { get; set; }

        /// <summary>
        /// 技术信息
        /// </summary>
        [Column("technology_info")]
        public string TechnologyJson { get; set; }
    }
}
