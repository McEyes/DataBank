using ITPortal.Core.Services;

using SqlSugar;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataManagement.Core.Entitys
{
    [Table("md_data_source")]
    public class DataSourceEntity : AuditEntity<long>
    {
        /// <summary>
        /// 资源类型:元数据类别
        /// </summary>
        public string DataCatalogId { get; set; }

        /// <summary>
        /// 数据源编码
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string? EnglishName { get; set; }

        /// <summary>
        /// 系统英文简称
        /// </summary>
        public string? NickName { get; set; }
        /// <summary>
        /// 业务系统类别/应用类型
        /// </summary>
        public string? SystemType { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string? OwnerDept { get; set; }
        /// <summary>
        /// SME NTID/ 负责人
        /// </summary>
        public string? SME { get; set; }
        /// <summary>
        /// SME DisplayName
        /// </summary>
        public string? SMEName { get; set; }

        /// <summary>
        /// 数据库类型/数据源类型
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 基础信息
        /// </summary>
        [SugarColumn(ColumnName ="base_info")]
        public string BaseJson { get; set; }

        /// <summary>
        /// 技术信息
        /// </summary>
        [SugarColumn(ColumnName = "technology_info")]
        public string TechnologyJson { get; set; }

    }
}
