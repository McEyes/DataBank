using ITPortal.Core.DistributedCache;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace ITPortal.Core
{
    public class DynamicRouteDocumentFilter : IDocumentFilter
    {
        private readonly IDistributedCacheService _cache;
        public DynamicRouteDocumentFilter(IDistributedCacheService cache)
        {
            _cache = cache;
        }
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (context.DocumentName.Equals("DataAssetManager")) return;
            var routeList = _cache.HashGetAll<RouteInfo>(DataAssetManagerConst.RouteRedisKey);
            foreach (var route in routeList)
            {
                var path = route.ApiServiceUrl;
                var config = route;
                if (context.DocumentName.Equals("DataAssetManager") || !path.Contains(context.DocumentName,StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }


                // 添加路径到 Swagger 文档
                swaggerDoc.Paths.Add(path, new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [config.ReqType] = new OpenApiOperation
                        {
                            Summary = config.ApiName,//文档简介
                            Description = config.Remark,//文档详细描述
                            Parameters = new List<OpenApiParameter>()
                        }
                    }
                });

                var operation = swaggerDoc.Paths[path].Operations[config.ReqType];
                // 添加参数到 Swagger 文档
                //添加 tags
                if (operation.Tags == null)
                {
                    operation.Tags = new List<OpenApiTag>();
                }
                if (config.ExecuteConfig != null)
                    operation.Tags.Add(new OpenApiTag() { Name = config.ExecuteConfig?.tableName });
                //if (config.ExecuteConfig != null)
                //{
                //    var apiNames = config.ApiName.Split('-');
                //    var index = 0;
                //    foreach (var tag in apiNames)
                //    {
                //        operation.Tags.Add(new OpenApiTag() { Name = tag });
                //        if (index++ > 2) break;
                //    }
                //}

                operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        Name = "pageSize",
                        In = ParameterLocation.Query,
                        Required = false,
                        Schema = new OpenApiSchema { Type = "int" },
                        Description = "分页大小",
                        AllowEmptyValue = true,
                        Example = new OpenApiInteger(1000)
                    }
                );
                operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        Name = "pageNum",
                        In = ParameterLocation.Query,
                        Required = false,
                        Schema = new OpenApiSchema { Type = "int" },
                        Description = "第几页",
                        AllowEmptyValue = true,
                        Example = new OpenApiInteger(1)
                    }
                );
                operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        Name = "total",
                        In = ParameterLocation.Query,
                        Required = false,
                        Schema = new OpenApiSchema { Type = "int" },
                        Description = "总数据数量，当从第二页分页查询时，需要传入第一页返回的总数量，用于校验，避免超限查询。",
                        AllowEmptyValue = true,
                        Example = new OpenApiInteger(1)
                    }
                );
                if (config.ReqParams == null || config.ReqParams.Count == 0)
                {
                    if (path.EndsWith("sqlQuery"))
                    {
                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "sqlText",
                                In = ParameterLocation.Query,
                                Required = false,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "sql查询语句",
                                Example = new OpenApiString($"select * from {config.ExecuteConfig.tableName}")
                            }
                        );
                    }
                    continue;
                }
                foreach (var param in config.ReqParams)
                {
                    operation.Parameters.Add(
                        new OpenApiParameter
                        {
                            Name = param.paramName,
                            In = ParameterLocation.Query,
                            Required = false,//param.nullable != "0",
                            AllowEmptyValue = true,
                            Schema = new OpenApiSchema { Type = MapParamTypeToSwaggerType(param.paramType) },
                            Description = param.paramComment,
                            Example = new OpenApiString(param.defaultValue)
                        }
                    );
                }
                if (config.ReqType == OperationType.Post)
                {
                    operation.RequestBody = new OpenApiRequestBody()
                    {
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            {
                                "application/json",
                                new OpenApiMediaType()
                                {
                                    Schema = new OpenApiSchema()
                                    {
                                        Type = "object",
                                        Properties = new Dictionary<string, OpenApiSchema>()
                                    }
                                }
                            }
                        },
                        Required = false
                    };
                }
            }
            
            
            //GetGroupTags(swaggerDoc);
            //Console.WriteLine(JSON.Serialize(paramTypeDict));
        }
        //private static ConcurrentDictionary<string, string> paramTypeDict = new ConcurrentDictionary<string, string>();
        private string MapParamTypeToSwaggerType(string paramType)
        {
            // 将数据库类型映射为 Swagger 类型
            //if (paramType != null) paramTypeDict.TryAdd(paramType, paramType);
            return paramType?.ToLower() switch
            {
                "varchar" => "string",
                "nvarchar" => "string",
                "ntext" => "string",
                "text" => "string",
                "json" => "string",
                "int" => "integer",
                "bigint" => "integer",
                "tinyint" => "integer",
                "smallint" => "integer",
                "numeric" => "integer",
                "int4" => "integer",
                "bit" => "bool",
                "datetimeoffset" => "date", // Swagger 中日期时间通常用字符串表示
                "timestamp without time zone" => "date", // Swagger 中日期时间通常用字符串表示
                "timestamp(0)" => "date", // Swagger 中日期时间通常用字符串表示
                "VARCHAR(50)" => "string", // Swagger 中日期时间通常用字符串表示
                _ => "string"
            };
        }

        //private string GetTableName(string tableId)
        //{
        //    if (string.IsNullOrWhiteSpace(tableId)) return "[TableName]";
        //    var dataSouce = _cache.HashGet<DataSourceEntity>(DataAssetManagerConst.DataTable_HashKey, tableId);
        //    if (dataSouce != null) return dataSouce.SourceName;
        //    else return "tableName";
        //}

        //private void GetGroupTags(OpenApiDocument swaggerDoc)
        //{

        //    swaggerDoc.Tags = new List<OpenApiTag>
        //        {
        //            new OpenApiTag
        //            {
        //                Name = "Users",
        //                Description = "与用户相关的 API 接口，包含用户的创建、查询、更新和删除操作。"
        //            },
        //            new OpenApiTag
        //            {
        //                Name = "Products",
        //                Description = "与产品相关的 API 接口，包含产品的列表展示、详情查看和库存管理等操作。"
        //            }
        //        };
        //}

        //public static GetXml()
        //{
        //    var frameworkPackageName = Reflect.GetAssemblyName(GetType());
        //    var projectXmlComments = App.Assemblies.Where(u => u.GetName().Name != frameworkPackageName).Select(t => t.GetName().Name);
        //    var externalXmlComments = App.ExternalAssemblies.Any() ? App.PathOfExternalAssemblies.Select(u => u.EndsWith(".dll") ? u[0..^4] : u) : Array.Empty<string>();
        //    XmlComments ??= projectXmlComments.Concat(externalXmlComments).ToArray();
        //}

    }

     
    //public static class DynamicRouteConfigurations
    //{
    //    public static readonly Dictionary<string, RouteInfo> Routes = new Dictionary<string, RouteInfo>();
    //}
}