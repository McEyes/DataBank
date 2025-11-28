
using ITPortal.Core;

using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Core
{
    public class RouteInfo
    {
        public const string DataAssetManagerRouteName = "services";
        public string Id { get; set; }

        public int? Status { get; set; }
        /// <summary>
        /// API名称
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// API版本
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// API路径
        /// </summary>
        public string ApiUrl { get; set; }
        /// <summary>
        /// API路径
        /// </summary>
        public string ApiServiceUrl
        {
            get { return $"/services/{ApiVersion}{ApiUrl}"; }
        }

        /// <summary>
        /// 请求类型
        /// </summary>
        private string _ReqMethod;
        public string ReqMethod
        {
            get { return _ReqMethod; }
            set
            {
                if (_ReqMethod != value)
                {
                    _ReqMethod = value;
                    _ReqType = MapMethodToOperationType(value);
                }
            }
        }
        /// <summary>
        /// 请求类型
        /// </summary>
        private OperationType _ReqType;
        public OperationType ReqType { get { return _ReqType; } }

        /// <summary>
        /// 返回格式
        /// </summary>
        public string ResType { get; set; }

        /// <summary>
        /// 数据源id
        /// </summary>
        public string SourceId { get; set; }
        /// <summary>
        /// 数据表Id，还需要关联查询，目前没有值
        /// </summary>
        public string TableId { get; set; }

        /// <summary>
        /// IP黑名单多个，隔开
        /// </summary>
        public string Deny { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 限流配置
        /// </summary>
        public RateLimit RateLimit { get; set; }


        /// <summary>
        /// 执行配置
        /// </summary>
        public ExecuteConfig ExecuteConfig { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public List<ReqParam> ReqParams { get; set; }

        /// <summary>
        /// 返回字段
        /// </summary>
        public List<ResParam> ResParams { get; set; }


        public static OperationType MapMethodToOperationType(string reqMethod)
        {
            // 将数据库类型映射为 Swagger 类型
            return reqMethod.ToLower() switch
            {
                "get" => OperationType.Get,
                "post" => OperationType.Post,
                "put" => OperationType.Put,
                "Delete" => OperationType.Delete,
                "options" => OperationType.Options,
                "head" => OperationType.Head,
                "patch" => OperationType.Patch,
                "trace" => OperationType.Trace,
                _ => OperationType.Get,
            };
        }
    }
}
