using DataAssetManager.DataApiServer.Web.Core;

using Furion;
using Furion.Schedule;
using Furion.UnifyResult;

using ITPortal.AuthServer.Core;
using ITPortal.AuthServer.Core.AccountService;
using ITPortal.Core.EvenBus;
using ITPortal.Core.Extensions;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json.Serialization;

using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Web.Core
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // 添加规范化提供器
            services.TryAddSingleton<IUnifyResultProvider, ResultProvider>();


            // 添加Windows身份验证（用于获取当前用户）
            //AddAdAuth(services);
            services.AddDefaultAuth();
            services.AddCorsAccessor();

            services.AddControllersWithViews();
            services.AddControllers(options =>
            {
                options.Filters.Add<ActionResultFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //时间格式化
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //忽略循环引用
                //options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            })
            .AddDynamicApiControllers()
            .AddDataValidation()
            .AddFriendlyException();
            //.AddView();
            services.AddSwaggerGenNewtonsoftSupport();



            services.AddSpecificationDocuments();
            //services.AddDynamicApiControllers();

            //// 注册自定义服务;
            //// 注册自定义授权处理程序
            services.AddLog();
            services.AddRedisCache();
            services.AddTaskQueue();
            services.AddSqlsugarSetup(App.Configuration);
            services.AddConsulConfig(App.Configuration);
            services.AddProxyService(App.Configuration);


            //启动后台服务
            services.AddSchedule(options =>
            {
                //options.AddJob<SynEmployeeInfoJob>();
                options.AddJob(App.EffectiveTypes.ScanToBuilders());
            });

            //注册 EventBus 服务
            services.AddCabEventBus();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<TraceApiLogMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCorsAccessor();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseInject();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseConsul();
        }


        /// <summary>
        /// 添加身份验证服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public  IServiceCollection AddAdAuth( IServiceCollection services)
        {
            // 配置授
            // 添加身份验证服务
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddNegotiate(options =>
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        var ldapOptions = App.GetConfig<LDAPOptions>("LDAP");
                        if (ldapOptions.MachineAccountName.IsNotNullOrWhiteSpace())
                        {
                            options.EnableLdap(settings =>
                            {
                                settings.Domain = ldapOptions.Domain;
                                settings.MachineAccountName = ldapOptions.MachineAccountName;
                                settings.MachineAccountPassword = ldapOptions.MachineAccountPassword;
                            });
                        }
                        else
                        {
                            options.EnableLdap(settings =>
                            {
                                settings.Domain = ldapOptions.Domain;
                            });
                        }
                    }
                    //// 允许从Windows凭据中提取角色信息
                    //options.Events = new NegotiateEvents
                    //{
                    //    OnAuthenticated = context =>
                    //    {
                    //        // 可在此处添加自定义声明
                    //        return Task.CompletedTask;
                    //    }
                    //};
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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

            //添加授权服务
            services.AddAuthorization(options =>
            {
                // 默认策略：允许两种认证方式
                options.DefaultPolicy = new AuthorizationPolicyBuilder(
                    NegotiateDefaults.AuthenticationScheme,
                    JwtBearerDefaults.AuthenticationScheme
                )
                .RequireAuthenticatedUser()
                .Build();

                // 仅Negotiate认证的策略
                options.AddPolicy("RequireNegotiate", policy =>
                    policy.RequireAuthenticatedUser()
                          .AddAuthenticationSchemes(NegotiateDefaults.AuthenticationScheme));

                // 仅JWT认证的策略
                options.AddPolicy("RequireJwt", policy =>
                    policy.RequireAuthenticatedUser()
                          .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
            });
            services.AddSingleton<IAuthorizationHandler, ITPortal.Core.DefaultJwtHandler>();

            return services;
        }

    }
}
