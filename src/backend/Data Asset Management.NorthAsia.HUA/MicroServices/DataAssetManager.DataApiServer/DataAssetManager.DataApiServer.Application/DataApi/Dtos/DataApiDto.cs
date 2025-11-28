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
    /// <summary>
    /// 数据API信息表 实体DTO
    /// </summary>
    public class DataApiDto : PageEntity<string>
    {
        /// <summary>
        /// API名称
        /// </summary>
        [Required(ErrorMessage = "API名称不能为空", AllowEmptyStrings = false)]
        public string ApiName { get; set; }

        /// <summary>
        /// API版本
        /// </summary>
        [Required(ErrorMessage = "API版本不能为空", AllowEmptyStrings = false)]
        public string ApiVersion { get; set; }

        /// <summary>
        /// API路径
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        [Required(ErrorMessage = "请求方式不能为空", AllowEmptyStrings = false)]
        public string ReqMethod { get; set; }

        /// <summary>
        /// 返回格式
        /// </summary>
        [Required(ErrorMessage = "返回格式不能为空", AllowEmptyStrings = false)]
        public string ResType { get; set; }

        /// <summary>
        /// IP黑名单多个用英文,隔开
        /// </summary>
        public string? Deny { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        //[Required(ErrorMessage = "数据源不能为空", AllowEmptyStrings = false)]
        public string SourceId { get; set; }

        /// <summary>
        /// 限流配置
        /// </summary>
        public RateLimit? RateLimit { get; set; }

        /// <summary>
        /// 执行配置
        /// </summary>
        public ExecuteConfig ExecuteConfig { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        //[Required(ErrorMessage = "请求参数不能为空")]
        //[MinLength(1, ErrorMessage = "请求参数长度不能少于{1}位")]
        public List<ReqParam>? ReqParams { get; set; }

        /// <summary>
        /// 返回参数
        /// </summary>
        //[Required(ErrorMessage = "返回字段不能为空")]
        //[MinLength(1, ErrorMessage = "返回字段长度不能少于{1}位")]
        public List<ResParam>? ResParams { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Required(ErrorMessage = "状态不能为空")]
        public string Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        public DateTimeOffset? CreateTime { get; set; }
    }
}
