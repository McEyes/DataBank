using Furion;
using Furion.UnifyResult;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.EvenBus;
using ITPortal.Core.Extensions;
using ITPortal.Core.Services;
using ITPortal.DataAssetFlow.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITPortal.DataAssetFlow.Web.Core.Extensions
{
    public static class InitExtensions
    {

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddTransient<CacheDbQueryFactory>();
            // 添加规范化提供器
            services.TryAddSingleton<IUnifyResultProvider, ResultProvider>();


            services.AddDefaultAuth();
            services.AddCorsAccessor();
            //services.AddSwaggerGen();
            services.AddController();
                        //.AddNewtonsoftJson()
                        //.AddInjectWithUnifyResult<ResultProvider>();
            services.AddSpecificationDocuments();
            services.AddDynamicApiControllers();

            //// 注册自定义服务;
            //// 注册自定义授权处理程序
            services.AddLog();
            services.AddRedisCache();
            services.AddTaskQueue();
            services.AddSqlsugarSetup(App.Configuration);
            services.AddConsulConfig(App.Configuration);
            services.AddProxyService(App.Configuration);


            //注册 EventBus 服务
            services.AddCabEventBus();
            return services;
        }
    }
}
