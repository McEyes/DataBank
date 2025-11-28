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
    [SugarTable("metadata_table_alias")]
    public class DataTableAlias
    {
        [SugarColumn(ColumnName = "table_name", IsPrimaryKey = true)]
        public string TableName { get; set; }
        [SugarColumn(ColumnName = "table_alias", IsPrimaryKey = true)]
        public string TableAlias { get; set; }
    }

}
