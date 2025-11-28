using ITPortal.Core;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据API信息表 实体DTO
    /// </summary>
    public class APIAutoGenParam
    {
        /// <summary>
        /// API 名称
        /// </summary>
        [Required(ErrorMessage = "API 名称不能为空", AllowEmptyStrings = false)]
        public string ApiName { set; get; }

        /// <summary>
        /// API 在自动创建是会在路径后面自动拼接公共路径后缀，只需要传定义好的路径即可。
        /// 如：apiUrl: /HR/Base/Sys_User => 生成 API 路径：
        /// 1. /HR/Base/Sys_User/query;  
        /// 2. /HR/Base/Sys_User/sqlQuery;
        /// </summary>
        [Required(ErrorMessage = "API 路径不能为空", AllowEmptyStrings = false)]
        public string ApiUrl { set; get; }
        /// <summary>
        /// 表ID
        /// </summary>
        [Required(ErrorMessage = "表ID", AllowEmptyStrings = false)]
        public string TableId { set; get; }
        /// <summary>
        /// API 备注
        /// </summary>
        public string? Remark { set; get; }
    }
}
