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

namespace DataAssetManager.DataApiServer.Application.DataCatalog.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [SugarTable("metadata_catalog_table_mapping")]
    public class DataCatalogTableMapping
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "catalog_id", IsPrimaryKey = true)]
        public string CatalogId { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "metadata_table_id", IsPrimaryKey = true)]
        public string TableId { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "sort")]
        public int? Sort { get; set; } = 100;

    }

}
