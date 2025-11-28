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

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据API信息表 实体DTO
    /// </summary>
    public  enum ApiState
    {
        /// <summary>
        /// 待发布
        /// </summary>
        [Description("待发布")]
        WAIT = 1,
        /// <summary>
        /// 已发布
        /// </summary>
        [Description("已发布")]
        RELEASE = 2,
        /// <summary>
        /// 已下线
        /// </summary>
        [Description("已下线")]
        CANCEL = 3
    }
}
