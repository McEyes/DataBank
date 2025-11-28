using DataAssetManager.DataApiServer.Web.Core;

using Furion;

using ITPortal.Core.EvenBus;
using ITPortal.Core.Extensions;
using ITPortal.ResMgrPlatform.Application.Dtos;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ITPortal.ResMgrPlatform.Web.Core
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsoleFormatter();
            //services.AddJwt<JwtHandler>();
            services.AddConsulConfig(App.Configuration);
            services.Configure<FileStorageOptions>(App.Configuration.GetSection("FileStorageOptions"));

            services.AddDefaultAuth();
            services.AddCorsAccessor();

            services.AddControllers()
                    .AddInjectWithUnifyResult();

            services.AddVirtualFileServer();
            services.AddLog();
            services.AddRedisCache();
            services.AddTaskQueue();
            services.AddProxyService(App.Configuration);
            services.AddCabEventBus();
            //services.AddApiServices();
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

            app.UseInject(string.Empty);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
