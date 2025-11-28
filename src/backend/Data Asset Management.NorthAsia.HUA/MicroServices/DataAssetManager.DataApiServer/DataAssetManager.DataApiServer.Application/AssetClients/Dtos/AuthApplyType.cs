using ITPortal.Core.Services;

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

namespace DataAssetManager.DataApiServer.Application.AssetClients.Dtos
{
    /// <summary>
    /// 资源授权申请类型
    /// </summary>
    public enum AuthApplyType
    {
        /// <summary>
        /// 个人申请
        /// </summary>
        [Description("个人申请")]
        Individual = 1,
        /// <summary>
        /// 应用申请
        /// </summary>
        [Description("应用申请")]
        Application = 2
    }
}
