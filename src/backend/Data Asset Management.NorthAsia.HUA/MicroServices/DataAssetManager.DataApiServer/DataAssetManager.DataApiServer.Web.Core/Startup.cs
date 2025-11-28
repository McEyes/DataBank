using DataAssetManager.DataApiServer.Web.Core.Extensions;
using DataAssetManager.DataTableServer.Application;

using Furion;

using ITPortal.Core.Extensions;

using Mapster;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using StackExchange.Profiling.Internal;

using System.Linq;

namespace DataAssetManager.DataApiServer.Web.Core
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)//, IServiceProvider serviceProvider
        {
            services.AddConsoleFormatter();

            // 注册自定义服务;
            services.AddApiServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)//, IServiceProvider serviceProvider
        {
            app.UseMiddleware<TraceApiLogMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(async c =>
            {
                c.SwaggerEndpoint("/swagger/DataAssetManager/swagger.json", "数据资产API");
                c.SwaggerEndpoint("/swagger/services/swagger.json", "数据资产services");
                c.RoutePrefix = string.Empty; // 将路径修改为根路径 "/"

                IDataCatalogService datalogService = app.ApplicationServices.GetService<IDataCatalogService>();
                var list = await datalogService.GetTopTopic();
                foreach (var item in list)
                {
                    if (item.Code.IsNullOrWhiteSpace()) c.SwaggerEndpoint("/swagger/services/swagger.json", "数据资产services");
                    else c.SwaggerEndpoint($"/swagger/{item.Code.Replace(" ", "").Replace("-", "_")}/swagger.json", item.Name);
                }
            });

            app.UseCorsAccessor();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseApiMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseInject("dataasset");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.InitFeed();
        }
    }
}
