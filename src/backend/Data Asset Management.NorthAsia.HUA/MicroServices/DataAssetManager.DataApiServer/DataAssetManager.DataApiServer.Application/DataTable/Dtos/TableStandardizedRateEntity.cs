using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Profiling.Internal;
using ITPortal.Core;
using ITPortal.Core.LightElasticSearch;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表 asset_standardized_rate_view
    /// </summary>
    [Serializable]
    [SugarTable("asset_standardized_rate_view")]
    public class TableStandardizedRateEntity 
    {

        /// <summary>
        /// 部门
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "部门")]
        [SugarColumn(ColumnName = "owner_depart")]
        public string Dept { get; set; }

        /// <summary>
        /// 数据表数量
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "数据表数量")]
        [SugarColumn(ColumnName = "table_count")]
        public int TableCount { get; set; }

        /// <summary>
        /// 字段数量
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "字段数量")]
        [SugarColumn(ColumnName = "column_count")]
        public int ColumnCount { get; set; }

        /// <summary>
        /// 主数据字段数量
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "主数据字段数量")]
        [SugarColumn(ColumnName = "masterdata_count")]
        public int MasterColumnCount { get; set; }
        /// <summary>
        /// 标准化
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "标准化")]
        [SugarColumn(ColumnName = "standardized_count")]
        public int StandardizedCount { get; set; }
        /// <summary>
        /// 标准化比率
        /// </summary>
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "标准化比率")]
        [SugarColumn(ColumnName = "standardized_rate")]
        public decimal StandardizedRate { get; set; }
    }

}
