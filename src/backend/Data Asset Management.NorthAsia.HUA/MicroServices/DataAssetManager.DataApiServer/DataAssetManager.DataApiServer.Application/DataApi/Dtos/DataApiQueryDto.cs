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

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    public class DataApiQueryDto : PageEntity<string>
    {
        public override string Id { get; set; }

        public int? Status { get; set; }
        /// <summary>
        /// API名称
        /// </summary>
        public string? ApiName { get; set; }

        /// <summary>
        /// API版本
        /// </summary>
        public string? ApiVersion { get; set; }

        /// <summary>
        /// API路径
        /// </summary>
        public string? ApiUrl { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string? ReqMethod { get; set; }

        /// <summary>
        /// 返回格式
        /// </summary>
        public string? ResType { get; set; }

        /// <summary>
        /// 数据源id
        /// </summary>
        public string? SourceId { get; set; }

        /// <summary>
        /// IP黑名单多个，隔开
        /// </summary>
        public string? Deny { get; set; }


        /// <summary>
        /// IP黑名单多个，隔开
        /// </summary>
        public string? ConfigType { get; set; }

    }
}
