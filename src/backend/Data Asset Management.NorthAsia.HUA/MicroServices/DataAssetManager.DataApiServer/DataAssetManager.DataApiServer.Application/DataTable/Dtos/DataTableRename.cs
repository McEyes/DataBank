using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataTable.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("metadata_table_rename")]
    public class DataTableRename
    {
        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "source_id", Length = 255)]
        public string SourceId { get; set; }
        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "source_name", Length = 255, IsPrimaryKey = true)]
        public string SourceName { get; set; }
        [SugarColumn(ColumnName = "table_name", IsPrimaryKey = true)]
        public string TableName { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string TabkeKey { get { return $"{SourceName}-{TableName}".ToLower(); } }
        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "new_source_id", Length = 255)]
        public string NewSourceId { get; set; }
        /// <summary>
        /// 所属数据源
        /// </summary>
        [SugarColumn(ColumnName = "new_source_name", Length = 255)]
        public string NewSourceName { get; set; }
        [SugarColumn(ColumnName = "new_table_name")]
        public string NewTableName { get; set; }
        [SugarColumn(ColumnName = "effect")]
        public bool? Effect { get; set; }
    }
}
