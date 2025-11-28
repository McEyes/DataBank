using Furion;
using Furion.Schedule;
using Furion.UnifyResult;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.EvenBus;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.Services;
using ITPortal.Search.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using StackExchange.Profiling.Internal;

using System.Threading.Tasks;
//using ITPortal.Core.Middleware;

namespace ITPortal.Search.Web.Core.Extensions
{
    public static class InitExtensions
    {

        public static IApplicationBuilder UseApiMiddleware(this IApplicationBuilder app)
        {
            //app.UseActionTracking();
            app.UseConsul(); 
            return app;
        }


        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddTransient<CacheDbQueryFactory>();
            // 添加规范化提供器
            services.TryAddSingleton<IUnifyResultProvider, ResultProvider>();

            services.AddDefaultAuth();
            services.AddCorsAccessor();
            services.AddController();

            // 注册自定义服务;
            // 注册自定义授权处理程序
            services.AddLog();
            services.AddRedisCache();
            services.AddTaskQueue();
            services.AddSqlsugarSetup(App.Configuration);
            services.AddConsulConfig(App.Configuration);
            services.AddProxyService(App.Configuration);

            //启动后台服务
            services.AddSchedule(options =>
            {
                options.AddJob(App.EffectiveTypes.ScanToBuilders());
            });

            // 注册 EventBus 服务
            services.AddCabEventBus();
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

            return services;
        }

    }
}
