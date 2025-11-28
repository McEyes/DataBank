using Furion.SpecificationDocument;
using Furion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Furion.DependencyInjection;
using DataAssetManager.DataTableServer.Application;
using StackExchange.Profiling.Internal;
using Microsoft.CodeAnalysis;
using ITPortal.Core;

namespace DataAssetManager.DataApiServer.Web.Core.Extensions
{
    public static class SpecificationDocumentBuilder
    {
        /// <summary>
        /// 所有分组默认的组名 Key
        /// </summary>
        private const string AllGroupsKey = "All Groups";

        /// <summary>
        /// 规范化文档配置
        /// </summary>
        private static readonly SpecificationDocumentSettingsOptions _specificationDocumentSettings;

        /// <summary>
        /// 应用全局配置
        /// </summary>
        private static readonly AppSettingsOptions _appSettings;

        /// <summary>
        /// 分组信息
        /// </summary>
        private static readonly IEnumerable<GroupInfoExtra> DocumentGroupExtras;

        /// <summary>
        /// 带排序的分组名
        /// </summary>
        private static readonly Regex _groupOrderRegex;

        /// <summary>
        /// 文档分组列表
        /// </summary>
        public static readonly IEnumerable<string> DocumentGroups;

        /// <summary>
        /// 构造函数
        /// </summary>
        static SpecificationDocumentBuilder()
        {
            //载入配置
            _specificationDocumentSettings = App.GetConfig<SpecificationDocumentSettingsOptions>("SpecificationDocumentSettings", true);
            _appSettings = App.Settings;

            //// 初始化常量
            _groupOrderRegex = new Regex(@"@(?<order>[0-9]+$)");
            GetActionGroupsCached = new ConcurrentDictionary<MethodInfo, IEnumerable<GroupInfoExtra>>();
            GetControllerGroupsCached = new ConcurrentDictionary<Type, IEnumerable<GroupInfoExtra>>();
            GetGroupOpenApiInfoCached = new ConcurrentDictionary<string, SpecificationOpenApiInfo>();
            //GetControllerTagCached = new ConcurrentDictionary<ControllerActionDescriptor, string>();
            //GetActionTagCached = new ConcurrentDictionary<ApiDescription, string>();

            //// 默认分组，支持多个逗号分割

            DocumentGroupExtras = new List<GroupInfoExtra>() { ResolveGroupExtraInfo(_specificationDocumentSettings.DefaultGroupName) };

            // 加载所有分组
            DocumentGroups = ReadGroups();
            //var gr = DocumentGroups.ToList();
        }

        /// <summary>
        /// 添加规范化文档服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configure">自定义配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddSpecificationDocuments(this IServiceCollection services, Action<SwaggerGenOptions> configure = default)
        {
            // 解决服务重复注册问题
            if (services.Any(u => u.ServiceType == typeof(IConfigureOptions<SchemaGeneratorOptions>)))
            {
                return services;
            }

            // 判断是否启用规范化文档
            if (App.Settings.InjectSpecificationDocument != true) return services;

            // 添加配置
            services.AddConfigurableOptions<SpecificationDocumentSettingsOptions>();
            services.AddEndpointsApiExplorer();

            // 添加Swagger生成器服务
            services.AddSwaggerGen(options => BuildGen(options, configure));

            // 添加 MiniProfiler 服务
            //AddMiniProfiler(services);

            return services;
        }


        /// <summary>
        /// Swagger 生成器构建
        /// </summary>
        /// <param name="swaggerGenOptions">Swagger 生成器配置</param>
        /// <param name="configure">自定义配置</param>
        internal static void BuildGen(SwaggerGenOptions swaggerGenOptions, Action<SwaggerGenOptions> configure = null)
        {
            // 创建分组文档
            CreateSwaggerDocs(swaggerGenOptions);

            //// 加载分组控制器和动作方法列表
            //LoadGroupControllerWithActions(swaggerGenOptions);

            //// 配置 Swagger OperationIds
            //ConfigureOperationIds(swaggerGenOptions);

            // 配置 Swagger SchemaId
            //ConfigureSchemaIds(swaggerGenOptions);

            //// 配置标签
            //ConfigureTagsAction(swaggerGenOptions);

            //// 配置 Action 排序
            //ConfigureActionSequence(swaggerGenOptions);

            //if (_specificationDocumentSettings.EnableXmlComments == true)
            //{
            //    // 加载注释描述文件
            //    LoadXmlComments(swaggerGenOptions);
            //}

            //配置授权
            ConfigureSecurities(swaggerGenOptions);

            //使得 Swagger 能够正确地显示 Enum 的对应关系
            if (_specificationDocumentSettings.EnableEnumSchemaFilter == true) swaggerGenOptions.SchemaFilter<EnumSchemaFilter>();

            // 修复 editor.swagger.io 生成不能正常处理 C# object 类型问题
            swaggerGenOptions.SchemaFilter<AnySchemaFilter>();

            // 添加 Action 操作过滤器
            swaggerGenOptions.OperationFilter<ApiActionFilter>();

            // 自定义配置
            configure?.Invoke(swaggerGenOptions);

            // 支持控制器排序操作
            if (_specificationDocumentSettings.EnableTagsOrderDocumentFilter == true) swaggerGenOptions.DocumentFilter<TagsOrderDocumentFilter>(); 
            swaggerGenOptions.DocumentFilter<DynamicRouteDocumentFilter>();
        }


        /// <summary>
        /// 配置授权
        /// </summary>
        /// <param name="swaggerGenOptions">Swagger 生成器配置</param>
        private static void ConfigureSecurities(SwaggerGenOptions swaggerGenOptions)
        {
            // 判断是否启用了授权
            if (_specificationDocumentSettings.EnableAuthorized != true || _specificationDocumentSettings.SecurityDefinitions.Length == 0) return;

            var openApiSecurityRequirement = new OpenApiSecurityRequirement();

            // 生成安全定义
            foreach (var securityDefinition in _specificationDocumentSettings.SecurityDefinitions)
            {
                // Id 必须定义
                if (string.IsNullOrWhiteSpace(securityDefinition.Id)
                    || swaggerGenOptions.SwaggerGeneratorOptions.SecuritySchemes.ContainsKey(securityDefinition.Id)) continue;

                // 添加安全定义
                var openApiSecurityScheme = securityDefinition as OpenApiSecurityScheme;
                swaggerGenOptions.AddSecurityDefinition(securityDefinition.Id, openApiSecurityScheme);

                // 添加安全需求
                var securityRequirement = securityDefinition.Requirement;

                // C# 9.0 模式匹配新语法
                if (securityRequirement is { Scheme.Reference: not null })
                {
                    securityRequirement.Scheme.Reference.Id ??= securityDefinition.Id;
                    openApiSecurityRequirement.Add(securityRequirement.Scheme, securityRequirement.Accesses);
                }
            }

            // 添加安全需求
            if (openApiSecurityRequirement.Count > 0)
            {
                swaggerGenOptions.AddSecurityRequirement(openApiSecurityRequirement);
            }
        }

        /// <summary>
        /// 读取所有分组信息
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> ReadGroups()
        {
            // 获取所有的控制器和动作方法
            var controllers = App.EffectiveTypes.Where(u => Penetrates.IsApiController(u));
            if (!controllers.Any())
            {
                var defaultGroups = new List<string>();

                return defaultGroups;
            }

            var actions = controllers.SelectMany(c => c.GetMethods().Where(u => IsApiAction(u, c)));

            // 合并所有分组
            var groupOrders = controllers.SelectMany(u => GetControllerGroups(u))
                .Union(
                    actions.SelectMany(u => GetActionGroups(u))
                )
                .Where(u => u != null && u.Visible)
                // 分组后取最大排序
                .GroupBy(u => u.Group)
                .Select(u => new
                {
                    Group = u.Key,
                    Order = u.Max(x => x.Order),
                    Visible = true
                });

            // 分组排序
            var groups = groupOrders
                .OrderByDescending(u => u.Order)
                .ThenBy(u => u.Group)
                .Select(u => u.Group)
                .Union(_specificationDocumentSettings.PackagesGroups);

            // 启用总分组功能
            //if (_specificationDocumentSettings.EnableAllGroups == true)
            //{
            //    groups = groups.Concat(new[] { AllGroupsKey });
            //}
            groups = groups.Concat(new[] { AllGroupsKey });

            return groups;
        }

        /// <summary>
        /// 获取控制器组缓存集合
        /// </summary>
        private static readonly ConcurrentDictionary<Type, IEnumerable<GroupInfoExtra>> GetControllerGroupsCached;
        /// <summary>
        /// 获取控制器分组列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<GroupInfoExtra> GetControllerGroups(Type type)
        {
            return GetControllerGroupsCached.GetOrAdd(type, Function);

            // 本地函数
            static IEnumerable<GroupInfoExtra> Function(Type type)
            {
                // 如果控制器没有定义 [ApiDescriptionSettings] 特性，则返回默认分组
                if (!type.IsDefined(typeof(ApiDescriptionSettingsAttribute), true)) return DocumentGroupExtras;

                // 读取分组
                var apiDescriptionSettings = type.GetCustomAttribute<ApiDescriptionSettingsAttribute>(true);
                if (apiDescriptionSettings.Groups == null || apiDescriptionSettings.Groups.Length == 0) return DocumentGroupExtras;

                // 处理分组额外信息
                var groupExtras = new List<GroupInfoExtra>();
                foreach (var group in apiDescriptionSettings.Groups)
                {
                    groupExtras.Add(ResolveGroupExtraInfo(group));
                }

                return groupExtras;
            }
        }

        /// <summary>
        /// <see cref="GetActionGroups(MethodInfo)"/> 缓存集合
        /// </summary>
        private static readonly ConcurrentDictionary<MethodInfo, IEnumerable<GroupInfoExtra>> GetActionGroupsCached;

        /// <summary>
        /// 获取动作方法分组列表
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns></returns>
        public static IEnumerable<GroupInfoExtra> GetActionGroups(MethodInfo method)
        {
            return GetActionGroupsCached.GetOrAdd(method, Function);

            // 本地函数
            static IEnumerable<GroupInfoExtra> Function(MethodInfo method)
            {
                // 如果动作方法没有定义 [ApiDescriptionSettings] 特性，则返回所在控制器分组
                if (!method.IsDefined(typeof(ApiDescriptionSettingsAttribute), true)) return GetControllerGroups(method.ReflectedType);

                // 读取分组
                var apiDescriptionSettings = method.GetCustomAttribute<ApiDescriptionSettingsAttribute>(true);
                if (apiDescriptionSettings.Groups == null || apiDescriptionSettings.Groups.Length == 0) return GetControllerGroups(method.ReflectedType);

                // 处理排序
                var groupExtras = new List<GroupInfoExtra>();
                foreach (var group in apiDescriptionSettings.Groups)
                {
                    groupExtras.Add(ResolveGroupExtraInfo(group));
                }

                return groupExtras;
            }
        }

        /// <summary>
        /// 是否是动作方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="ReflectedType">声明类型</param>
        /// <returns></returns>
        public static bool IsApiAction(MethodInfo method, Type ReflectedType)
        {
            // 不是非公开、抽象、静态、泛型方法
            if (!method.IsPublic || method.IsAbstract || method.IsStatic || method.IsGenericMethod) return false;

            // 如果所在类型不是控制器，则该行为也被忽略
            if (method.ReflectedType != ReflectedType || method.DeclaringType == typeof(object)) return false;

            return true;
        }

        /// <summary>
        /// 解析分组附加信息
        /// </summary>
        /// <param name="group">分组名</param>
        /// <returns></returns>
        private static GroupInfoExtra ResolveGroupExtraInfo(string group)
        {
            string realGroup;
            var order = 0;

            if (!_groupOrderRegex.IsMatch(group)) realGroup = group;
            else
            {
                realGroup = _groupOrderRegex.Replace(group, "");
                order = int.Parse(_groupOrderRegex.Match(group).Groups["order"].Value);
            }

            var groupOpenApiInfo = GetGroupOpenApiInfo(realGroup);
            return new GroupInfoExtra()
            {
                Group = realGroup,
                Order = groupOpenApiInfo.Order ?? order,
                Visible = groupOpenApiInfo.Visible ?? true
            };
        }
        /// <summary>
        /// 创建分组文档
        /// </summary>
        /// <param name="swaggerGenOptions">Swagger生成器对象</param>
        private static void CreateSwaggerDocs(SwaggerGenOptions swaggerGenOptions)
        {
            foreach (var group in DocumentGroups)
            {
                if (swaggerGenOptions.SwaggerGeneratorOptions.SwaggerDocs.ContainsKey(group)) continue;

                var groupOpenApiInfo = GetGroupOpenApiInfo(group) as OpenApiInfo;
                swaggerGenOptions.SwaggerDoc(group, groupOpenApiInfo);
            }
            var list = App.GetService<IDataCatalogService>().AllFromCache().Result;
            //var dataApiService = ServiceProviderServiceExtensions.GetRequiredService<IDataTableService>(serviceProvider);
            //var tableInfoList = await dataApiService.AllFromCache();
            foreach (var item in list)
            {
                if (item.Code.IsNullOrWhiteSpace()) continue;
                swaggerGenOptions.SwaggerDoc(item.Code.Replace(" ", ""), new OpenApiInfo
                {
                    Title = item.Name,
                    Description = item.Remark,
                    Contact = new OpenApiContact { Name = "数据资产团队", Email = "yang_li9954@jabil.com" },
                    Version = "v1"
                });
            }
        }
        /// <summary>
        /// 获取所有的规范化分组信息
        /// </summary>
        /// <returns></returns>
        public static List<SpecificationOpenApiInfo> GetOpenApiGroups()
        {
            var openApiGroups = new List<SpecificationOpenApiInfo>();
            foreach (var group in DocumentGroups)
            {
                openApiGroups.Add(GetGroupOpenApiInfo(group));
            }

            return openApiGroups;
        }
        /// <summary>
        /// 获取分组信息缓存集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, SpecificationOpenApiInfo> GetGroupOpenApiInfoCached;

        /// <summary>
        /// 获取分组配置信息
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static SpecificationOpenApiInfo GetGroupOpenApiInfo(string group)
        {
            return GetGroupOpenApiInfoCached.GetOrAdd(group, Function);

            // 本地函数
            static SpecificationOpenApiInfo Function(string group)
            {
                // 替换路由模板
                var routeTemplate = _specificationDocumentSettings.RouteTemplate.Replace("{documentName}", Uri.EscapeDataString(group));
                if (!string.IsNullOrWhiteSpace(_specificationDocumentSettings.ServerDir))
                {
                    routeTemplate = _specificationDocumentSettings.ServerDir + "/" + routeTemplate;
                }

                // 处理虚拟目录问题
                var template = $"{_appSettings.VirtualPath}/{routeTemplate}";

                var groupInfo = _specificationDocumentSettings.GroupOpenApiInfos.FirstOrDefault(u => u.Group == group);
                if (groupInfo != null)
                {
                    //groupInfo.RouteTemplate = template;
                    groupInfo.Title ??= group;
                }
                else
                {
                    groupInfo = new SpecificationOpenApiInfo { Group = group,
                        //RouteTemplate = template 
                    };
                }

                // 处理外部定义
                var groupKey = "[openapi:{0}]";
                if (App.Configuration.Exists(string.Format(groupKey, group)))
                {
                    SetProperty<int>(group, nameof(SpecificationOpenApiInfo.Order), value => groupInfo.Order = value);
                    SetProperty<bool>(group, nameof(SpecificationOpenApiInfo.Visible), value => groupInfo.Visible = value);
                    //SetProperty<string>(group, nameof(SpecificationOpenApiInfo.RouteTemplate), value => groupInfo.RouteTemplate = value);
                    SetProperty<string>(group, nameof(SpecificationOpenApiInfo.Title), value => groupInfo.Title = value);
                    SetProperty<string>(group, nameof(SpecificationOpenApiInfo.Description), value => groupInfo.Description = value);
                    SetProperty<string>(group, nameof(SpecificationOpenApiInfo.Version), value => groupInfo.Version = value);
                    SetProperty<Uri>(group, nameof(SpecificationOpenApiInfo.TermsOfService), value => groupInfo.TermsOfService = value);
                    SetProperty<OpenApiContact>(group, nameof(SpecificationOpenApiInfo.Contact), value => groupInfo.Contact = value);
                    SetProperty<OpenApiLicense>(group, nameof(SpecificationOpenApiInfo.License), value => groupInfo.License = value);
                }

                return groupInfo;
            }
        }

        /// <summary>
        /// 设置额外配置的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group"></param>
        /// <param name="propertyName"></param>
        /// <param name="action"></param>
        private static void SetProperty<T>(string group, string propertyName, Action<T> action)
        {
            var propertyKey = string.Format("[openapi:{0}]:{1}", group, propertyName);
            if (App.Configuration.Exists(propertyKey))
            {
                var value = App.GetConfig<T>(propertyKey);
                action?.Invoke(value);
            }
        }
    }
    /// <summary>
    /// 分组附加信息
    /// </summary>
    [SuppressSniffer]
    public sealed class GroupInfoExtra
    {
        /// <summary>
        /// 分组名
        /// </summary>
        public string Group { get; internal set; }

        /// <summary>
        /// 分组排序
        /// </summary>
        public int Order { get; internal set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool Visible { get; internal set; }
    }
}
