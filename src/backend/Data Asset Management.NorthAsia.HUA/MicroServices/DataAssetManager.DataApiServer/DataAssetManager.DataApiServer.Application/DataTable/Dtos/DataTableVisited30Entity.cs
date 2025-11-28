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
    ///30天api的访问数量
    /// </summary>
    [SugarTable("asset_table_visited_30")]
    public class DataTableVisited30Entity
    {
        //public string Id { get; set; }
        [SugarColumn(ColumnName = "table_id")]
        public string TableId { get; set; }
        [SugarColumn(ColumnName = "visited_times")]
        public int VisitedTimes { get; set; }
    }
}
