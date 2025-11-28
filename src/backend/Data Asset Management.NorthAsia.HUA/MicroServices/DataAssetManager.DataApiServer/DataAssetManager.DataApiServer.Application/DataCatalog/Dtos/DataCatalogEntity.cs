using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.Services;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [ElasticIndexName("DataCatalog", "DataAsset")]
    [SugarTable("asset_catalog")]
    public class DataCatalogEntity : AuditEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "ctl_id", IsPrimaryKey = true)]
        public override string Id { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "name")]
        public string? Name { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "code")]
        public string? Code { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "parent_ctl_id")]
        public string? ParentCtlId { get; set; }
        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public string? ParentName { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "remark")]
        public string? Remark { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "sort")]
        public int? Sort { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<DataCatalogEntity> Children { get; set; } = new List<DataCatalogEntity>();

    }

}
