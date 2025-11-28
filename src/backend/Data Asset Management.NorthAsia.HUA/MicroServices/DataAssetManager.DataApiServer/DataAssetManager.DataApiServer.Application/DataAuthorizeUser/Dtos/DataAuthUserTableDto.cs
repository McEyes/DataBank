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
    public class DataAuthUserTableDto
    {
        /// <summary>
        /// 表操作规则配置
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string? TableName { get; set; }
    }

}
