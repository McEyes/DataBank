using DataAssetManager.DataApiServer.Application;
using DataAssetManager.DataApiServer.Application.Services;
using DataAssetManager.DataApiServer.Core;
using DataAssetManager.DataTableServer.Application;

using Furion;
using Furion.HttpRemote.Extensions;
using Furion.Schedule;
using Furion.SpecificationDocument;
using Furion.UnifyResult;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Shapeless;

using StackExchange.Profiling.Internal;

using System.Collections.Generic;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Net.NetworkInformation;
using DataAssetManager.DataApiServer.Application.DataApi;
using DataAssetManager.DataApiServer.Application.EventSub;
using ITPortal.Core.EvenBus;
using Microsoft.Extensions.Configuration;
//using ITPortal.Core.Middleware;

namespace DataAssetManager.DataApiServer.Web.Core.Extensions
{
    public static class InitExtensions
    {

        public static IApplicationBuilder UseApiMiddleware(this IApplicationBuilder app)
        {
            //app.UseActionTracking();
            app.UseMiddleware<AuthorizeMiddleware>();
            app.UseMiddleware<RateLimitMiddleware>();
            app.UseMiddleware<BlackWhiteMiddleware>();
            app.UseMiddleware<DynamicRouteMiddleware>();
            app.UseConsul();
            return app;
        }


        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            //services.AddSingleton<SwaggerDynamicConfigService>();
            services.AddTransient<CacheDbQueryFactory>();
            services.AddTransient<ApiSqlServiceHandler>();
            services.AddTransient<ApiFormServiceHandler>();
            // 添加规范化提供器
            services.TryAddSingleton<IUnifyResultProvider, ResultProvider>();

            services.AddDataAssetAuth();
            services.AddCorsAccessor();
            services.AddController();

            // 注册自定义服务;
            // 注册自定义授权处理程序
            services.AddLog();
            services.AddRedisCache();
            services.AddTaskQueue();
            services.AddSqlsugarSetup(App.Configuration);
            services.AddSwagger();
            services.AddConsulConfig(App.Configuration);
            services.AddProxyService(App.Configuration);

            //启动后台服务
            services.AddSchedule(options =>
            {
                options.AddJob(App.EffectiveTypes.ScanToBuilders());
            });

            // 注册 EventBus 服务
            services.AddCabEventBus();
            //services.AddEventBus(builder =>
            //{
            //    // 注册 ToDo 事件订阅者
            //    //builder.AddSubscriber<CacheRefreshEventSubscriber>();

            //    //// 通过类型注册，Furion 4.2.1+ 版本
            //    //builder.AddSubscriber(typeof(ToDoEventSubscriber));

            //    //// 批量注册事件订阅者
            //    //builder.AddSubscribers(ass1, ass2, ....);
            //});
            return services;
        }
        /// <summary>
        /// 添加规范化提供器
        /// </summary>
        /// <typeparam name="TUnifyResultProvider"></typeparam>
        /// <param name="services"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnifyProvider<TUnifyResultProvider>(this IServiceCollection services, string providerName)
            where TUnifyResultProvider : class, IUnifyResultProvider
        {
            providerName ??= string.Empty;

            var providerType = typeof(TUnifyResultProvider);

            // 添加规范化提供器
            services.TryAddSingleton(providerType, providerType);

            // 获取规范化提供器模型，不能为空
            //var resultType = providerType.GetCustomAttribute<UnifyModelAttribute>().ModelType;

            // 创建规范化元数据
            //var metadata = new 
            //{
            //    ProviderName = providerName,
            //    ProviderType = providerType,
            //    ResultType = resultType
            //};

            // 添加或替换规范化配置
            //UnifyContext.UnifyProviders.AddOrUpdate(providerName, _ => metadata, (_, _) => metadata);

            return services;
        }

        public static IApplicationBuilder InitFeed(this IApplicationBuilder app)
        {
            var hostUrl = App.GetConfig<string>("RemoteApi:DataAssetHostUrl");
            if (!hostUrl.IsNullOrWhiteSpace()) DataAssetManagerConst.HostUrl = hostUrl;
            //启动数据
            Task.Run(() =>
            {
                App.GetService<IDataApiService>().InitRoutes(true);
                App.GetService<IDataApiService>().AllFromCache(true);
            });
            Task.Run(() =>
            {
                App.GetService<IDataTableService>().InitRedisHash(true);
            });
            Task.Run(() =>
            {
                App.GetService<IDataTableService>().InitTableUserFromCache(true);
            });
            Task.Run(() =>
            {
                App.GetService<IAssetClientsService>().InitClientScopes(true);
            });
            //Task.Run(async () =>
            //{
            //    await Task.Delay(1000);
            //    await App.GetService<EmployeeProxyService>().AllEmployeeAsync(true);
            //});
            return app;
        }



        private static HashSet<string> swaggerDocHash = new System.Collections.Generic.HashSet<string>();
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("DataAssetManager", new OpenApiInfo
                {
                    Title = "数据资产API",
                    Description = "数据资产API服务中心，包含数据资产对外提供的所有动态api",
                    Contact = new OpenApiContact { Name = "数据资产团队", Email = "yang_li9954@jabil.com" },
                    Version = "v1"
                });
                c.SwaggerDoc("services", new OpenApiInfo
                {
                    Title = "数据资产services",
                    Description = "数据资产API服务中心，包含数据资产对外提供的所有动态api服务",
                    Contact = new OpenApiContact { Name = "数据资产团队", Email = "yang_li9954@jabil.com" },
                    Version = "v1.0.0"
                });

                IDataCatalogService datalogService = App.GetService<IDataCatalogService>();
                var list = datalogService.GetTopTopic().Result;
                foreach (var item in list)
                {
                    if (!item.Code.IsNullOrWhiteSpace())
                    {
                        var key = item.Code.Replace(" ", "").Replace("-", "_");
                        if (swaggerDocHash.Contains(key)) continue;
                        swaggerDocHash.Add(key);
                        c.SwaggerDoc(key, new OpenApiInfo
                        {
                            Title = $"数据资产services-{item.ParentName}-{item.Name}",
                            Description = $"数据资产API服务中心，包含数据资产对外提供的{item.ParentName}-{item.Name}动态api服务,{item.Remark}",
                            Contact = new OpenApiContact { Name = "数据资产团队", Email = "yang_li9954@jabil.com" },
                            Version = "v1.0.0"
                        });
                    }
                }

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    //var apiService = App.GetService<IDataApiService>();
                    //var apiDict = apiService.GetTopTopicApiList().Result;
                    // 隐藏特定的 API
                    if (docName.Equals("DataAssetManager"))
                    {
                        return !apiDesc.RelativePath.Contains($"services");
                    }
                    else if (docName.Equals("services"))
                    {
                        return apiDesc.RelativePath.Contains($"services");
                    }
                    //else if (apiDict.ContainsKey(docName))
                    //{// 根据分类关联的表面显示
                    //    //return false;
                    //    return apiDict[docName].Contains(apiDesc.RelativePath);
                    //}
                    else
                    {// 根据分类关联的表面显示
                        //return false;
                        return apiDesc.RelativePath.Contains($"services") && apiDesc.RelativePath.Contains(docName);
                    }
                });
                //c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                //{
                //    Type = SecuritySchemeType.OAuth2,
                //    Flows = new OpenApiOAuthFlows
                //    {
                //        AuthorizationCode = new OpenApiOAuthFlow
                //        {
                //            AuthorizationUrl = new Uri("https://example.com/oauth/authorize"),
                //            TokenUrl = new Uri("https://example.com/oauth/token"),
                //            Scopes = new Dictionary<string, string>
                //                {
                //                    { "read", "Read access" },
                //                    { "write", "Write access" }
                //                }
                //        }
                //    }
                //});
                // 添加Bearer认证  
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // 为API添加Bearer认证需求  
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            },
                            new List<string>()
                        }
                    });
                c.DocumentFilter<DynamicRouteDocumentFilter>();
                //c.DocumentFilter<TagsOrderDocumentFilter>();
                // 获取 XML 文档文件路径
                var xmlFile = $"DataAssetManager.DataApiServer.Application.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 启用 XML 注释
                c.IncludeXmlComments(xmlPath);
            });
            return services;
        }
    }
}
