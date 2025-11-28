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
    /// 3 Confidential, 2 Internal Use, 1 Public, 4 Restricted Confidential
    /// 3机密,2	内部使用级,1	公众级,4	受限机密
    /// </summary>
    public class TableApiVisitStatics
    {
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "表名", Index = 1)]
        public string TableCode { get; set; }

        [MiniExcelLibs.Attributes.ExcelColumn(Name = "表名描述", Index = 2)]
        public string TableName { get; set; }
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "部门", Index = 0)]
        public string Dept { get; set; }
        [MiniExcelLibs.Attributes.ExcelColumn(Name = "近30天访问数量", Index = 3)]
        public int Count { get; set; }

    }

}
