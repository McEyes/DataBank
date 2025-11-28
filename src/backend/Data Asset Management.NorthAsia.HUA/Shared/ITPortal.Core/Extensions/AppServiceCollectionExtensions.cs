using Consul;

using DataAssetManager.DataApiServer.Web.Core;

using Furion;
using Furion.Authorization;
using Furion.HttpRemote.Extensions;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.ProxyApi;
using ITPortal.Extension.System.DateFormatExtensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shapeless;

using System.Reflection;
using System.Text;
using System.Text.Json;

namespace ITPortal.Core.Extensions
{
    public static class AppServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, Action<RedisCacheOptions> configure = null)
        {
            services.Configure<RedisCacheOptions>(option =>
            {
                if (configure != null)
                {
                    configure?.Invoke(option);
                }
                else
                {
                    var config = App.GetConfig<RedisCacheOptions>("Redis");
                    option.Configuration = config.Configuration;
                    option.InstanceName = config.InstanceName;
                }
            });
            //services.AddStackExchangeRedisCache(option =>
            //{
            //    var config = App.GetConfig<RedisCacheOptions>("Redis");
            //    option.Configuration = config.Configuration;
            //    option.InstanceName = config.InstanceName;
            //});
            services.AddCacheManager(App.Configuration);
            //services.Add(ServiceDescriptor.Singleton<IDistributedCacheService, RedisCacheService>());
            return services;
        }

        public static IServiceCollection AddLog(this IServiceCollection services)
        {
            services.AddLogging();
            ///生产日志
            services.AddFileLogging("logs/application/application-{0:yyyy}-{0:MM}-{0:dd}.log", options =>
            {
                options.FileNameRule = fileName =>
                {
                    return string.Format(fileName, DateTimeOffset.Now);
                };
                options.WriteFilter = (logMsg) =>
                {
                    return logMsg.LogLevel == Microsoft.Extensions.Logging.LogLevel.Information;
                };
            });
            //异常日志
            services.AddFileLogging("logs/error/error-{0:yyyy}-{0:MM}-{0:dd}.log", options =>
            {
                options.FileNameRule = fileName =>
                {
                    return string.Format(fileName, DateTimeOffset.Now);
                };
                options.WriteFilter = (logMsg) =>
                {
                    return logMsg.LogLevel == Microsoft.Extensions.Logging.LogLevel.Error;
                };
            });
            return services;
        }
        /// <summary>
        /// 添加身份验证服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDataAssetAuth(this IServiceCollection services)
        {
            // 添加身份验证服务
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var config = App.Configuration;
                    var jwtSettings = config.GetSection("JWTSettings");

                    // 读取配置信息
                    var issuer = jwtSettings["ValidIssuer"];
                    var audience = jwtSettings["ValidAudience"];
                    var secretKey = jwtSettings["IssuerSigningKey"] ?? "";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
                    };
                });
//#if STG
//            // 添加授权服务
//            services.AddAuthorization();
//#else
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                //options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));

                //// 配置自定义授权策略
                //options.AddPolicy("ITDepartmentPolicy", policy =>
                //{
                //    policy.Requirements.Add(new CustomAuthorizationRequirement("IT"));
                //});
            });
            services.AddSingleton<IAuthorizationHandler, DataAssetJwtHandler>();
//#endif
            return services;
        }

        /// <summary>
        /// 添加身份验证服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuth<AuthorizeHandlerType>(this IServiceCollection services)where AuthorizeHandlerType : AppAuthorizeHandler
        {
            // 添加身份验证服务
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var config = App.Configuration;
                    var jwtSettings = config.GetSection("JWTSettings");

                    // 读取配置信息
                    var issuer = jwtSettings["ValidIssuer"];
                    var audience = jwtSettings["ValidAudience"];
                    var secretKey = jwtSettings["IssuerSigningKey"] ?? "";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
                    };
                });
#if DEBUG
            // 添加授权服务
            services.AddAuthorization();
#else
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                //options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));

                //// 配置自定义授权策略
                //options.AddPolicy("ITDepartmentPolicy", policy =>
                //{
                //    policy.Requirements.Add(new CustomAuthorizationRequirement("IT"));
                //});
            });
            services.AddSingleton<IAuthorizationHandler, AuthorizeHandlerType>();
#endif
            return services;
        }


        /// <summary>
        /// 添加身份验证服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultAuth(this IServiceCollection services)
        {
            // 添加Windows身份验证（用于获取当前用户）
           //services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
           //    .AddNegotiate();

            // 配置授
            // 添加身份验证服务
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var config = App.Configuration;
                    var jwtSettings = config.GetSection("JWTSettings");

                    // 读取配置信息
                    var issuer = jwtSettings["ValidIssuer"];
                    var audience = jwtSettings["ValidAudience"];
                    var secretKey = jwtSettings["IssuerSigningKey"] ?? "";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
                    };
                });
//#if DEBUG || STG
            //添加授权服务
            services.AddAuthorization();
            services.AddSingleton<IAuthorizationHandler, DefaultJwtHandler>();
            //#else
            //            services.AddAuthorization(options =>
            //            {
            //                options.FallbackPolicy = new AuthorizationPolicyBuilder()
            //                    .RequireAuthenticatedUser()
            //                    .Build();
            //                //options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));

            //                //// 配置自定义授权策略
            //                //options.AddPolicy("ITDepartmentPolicy", policy =>
            //                //{
            //                //    policy.Requirements.Add(new CustomAuthorizationRequirement("IT"));
            //                //});
            //            });
            //            services.AddSingleton<IAuthorizationHandler, DefaultJwtHandler>();
            //#endif
            return services;
        }

        public static IServiceCollection AddController(this IServiceCollection services)
        {
            ConfigureGlobalJsonSettings();
            services.AddControllers(options =>
            {
                options.Filters.Add<ActionResultFilter>();
            })
            //services.AddControllers().AddInjectWithUnifyResult<PortalResultProvider>()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
                // 使用 System.Text.Json
                //options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new CustomDateTime2Converter());
                options.JsonSerializerOptions.Converters.Add(new CustomDateTimeOffsetConverter());
                options.JsonSerializerOptions.Converters.Add(new CustomDateTimeOffset2Converter());

                //// 设置日期格式（例如："yyyy-MM-dd HH:mm:ss"）
                //options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //时间格式化
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //忽略循环引用
                //options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                // 添加自定义日期转换器
                options.SerializerSettings.Converters.Add(new DefaultDateOnlyConverter());
                options.SerializerSettings.Converters.Add(new DefaultDateConverter());
                options.SerializerSettings.Converters.Add(new DefaultDate2Converter());
                options.SerializerSettings.Converters.Add(new UtcDateTimeOffsetConverter());
                options.SerializerSettings.Converters.Add(new UtcDateTimeOffset2Converter());
            })
            .AddDynamicApiControllers()
            .AddDataValidation()
            .AddFriendlyException();
            services.AddSwaggerGenNewtonsoftSupport();
            return services;
        }

        public static void ConfigureGlobalJsonSettings()
        {
            // 创建全局默认的序列化设置
            JsonSerializerSettings globalSettings = new JsonSerializerSettings();
            globalSettings.Converters.Add(new DefaultDateOnlyConverter());
            globalSettings.Converters.Add(new DefaultDateConverter());
            globalSettings.Converters.Add(new UtcDateTimeOffsetConverter());
            globalSettings.Converters.Add(new UtcDateTimeOffset2Converter());

            // 设置为全局默认
            JsonConvert.DefaultSettings = () => globalSettings;
        }

        public static IServiceCollection AddConsulConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = configuration.GetValue<string>("Consul:Host");
                consulConfig.Address = new Uri(address);
                consulConfig.Token = configuration.GetValue<string>("Consul:Token");
            }));
            services.AddHealthChecks();
            return services;
        }

        public static IServiceCollection AddProxyService(this IServiceCollection services, IConfiguration configuration=null)
        {
            services.AddTransient<BaseProxyService>();
            services.AddTransient<TrackLogProxyService>();
            services.AddTransient<EmployeeProxyService>();
            services.AddTransient<FlowApplyProxyService>();
            services.AddTransient<TopicDocumentProxyService>();
            services.AddTransient<MyEmployeeProxyService>();
            services.AddServiceDiscovery();
            services.AddHttpRemote(options =>
            {
                // 注册单个 HTTP 声明式请求接口
                //options.AddHttpDeclarative<IEmployeeService>();

                // 扫描程序集批量注册 HTTP 声明式请求接口（推荐此方式注册）
                options.AddHttpDeclarativeFromAssemblies([Assembly.GetEntryAssembly()]);
                options.AddHttpDeclarativeExtractorFromAssemblies([Assembly.GetEntryAssembly()]);
                options.AddHttpContentConverters(() => [new ClayContentConverter()]);
            }).ConfigureOptions(options => { options.JsonSerializerOptions.Converters.Add(new ClayJsonConverter()); })
            .ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddServiceDiscovery();
                clientBuilder.AddProfilerDelegatingHandler();
            });

            return services;
        }
    }

}
