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

namespace DataAssetManager.DataApiServer.Application.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class DataCatalogTableMappingDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string CatalogId { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        public string TableId { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        public int? Sort { get; set; }

    }

}
