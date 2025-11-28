using DataAssetManager.DataApiServer.Web.Core;

using Furion;
using Furion.HttpRemote.Extensions;
using Furion.Shapeless;

using ITPortal.Core;
using ITPortal.Core.EvenBus;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.Services;
using ITPortal.Search.Core;
using ITPortal.Search.Web.Core.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Reflection;

namespace ITPortal.Search.Web.Core
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsoleFormatter();
            services.AddJwt<DefaultJwtHandler>();
            //services.AddTransient<IUnifyResultProvider, PortalResultProvider>();

            services.AddCorsAccessor();
            //services.AddHealthChecks();
            services.AddConsulConfig(App.Configuration);

            services.AddControllers()
                        .AddNewtonsoftJson()
                    .AddInjectWithUnifyResult<ResultProvider>();

            services.AddRedisCache();
            services.AddTaskQueue();
            services.AddSqlsugarSetup(App.Configuration);
            services.AddUserSqlsugarSetup(App.Configuration);

            services.AddProxyService();
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
            //注册 EventBus 服务
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<TraceApiLogMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseConsul();

            app.UseCorsAccessor();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseInject("ITPortalSearch");


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
