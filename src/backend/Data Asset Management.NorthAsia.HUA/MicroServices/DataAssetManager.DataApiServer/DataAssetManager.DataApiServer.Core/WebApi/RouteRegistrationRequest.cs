using Microsoft.OpenApi.Models;

using System.Collections.Generic;

namespace DataAssetManager.DataApiServer.Core
{
    public class RouteRegistrationRequest
    {
        public string ApiUrl { get; set; }
        public string Description { get; set; } = "文档描述";
        public string Summary { get; set; } = "文档简介";
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string> { { "DataAssetManager", "数据资产" } };
        public OperationType OperationType { get; set; } = OperationType.Get;
        public string Status { get; set; }
        public ApiConfig SqlConfig { get; set; }
        public LimitConfig Limit { get; set; }
        public List<RequestParam> ReqJson { get; set; }
    }

    public class ApiConfig
    {
        public string ConnectionString { get; set; }
        public string SqlQuery { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
        public string DbType { get; set; }
    }
}