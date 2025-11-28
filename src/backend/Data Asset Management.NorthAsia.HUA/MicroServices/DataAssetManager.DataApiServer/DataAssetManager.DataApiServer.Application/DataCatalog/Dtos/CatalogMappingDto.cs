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

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class CatalogMappingDto 
    {
        /// <summary>
        /// 主题域ID
        /// </summary>
        [Required(ErrorMessage = "主题域ID不能为空", AllowEmptyStrings = false)]
        public string  CtlId;

        /// <summary>
        /// 表信息
        /// </summary>
        public List<DataTableEntity> MetadataTableDtoList;
    }

}
