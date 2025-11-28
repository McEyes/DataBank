using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Consul;

namespace DataTopicStore.Web.Core.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void AddConsul(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var consulAddress = configuration.GetValue<string>("Consul:Address");
            bool isEnabled = configuration.GetValue<bool>("Consul:Enabled");
            if (!isEnabled) return;

            services.AddSingleton<IConsulClient>(provider =>
            new ConsulClient(config =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                config.Address = new Uri(consulAddress);
            }));
        }

        public static void UseConsul(this IApplicationBuilder app)
        {
            var appLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            using var scope = app.ApplicationServices.CreateScope();
            var configuration = scope.ServiceProvider.GetService<IConfiguration>();

            bool isEnabled = configuration.GetValue<bool>("Consul:Enabled");
            var serviceName = configuration.GetValue<string>("Consul:Service");
            var serviceId = configuration.GetValue<string>("Consul:ServiceID");
            var appString = configuration.GetValue<string>("Consul:SelfUrl");
            Uri appUrl = new Uri(appString, UriKind.Absolute);

            if (!isEnabled)
                return;

            var client = scope.ServiceProvider.GetService<IConsulClient>();
            var consulServiceRegistration = new AgentServiceRegistration
            {
                Name = serviceName,
                ID = serviceId,
                Address = appUrl.Host,
                Port = appUrl.Port,
                Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromHours(1),//服务停止多久后注销
                    Interval = TimeSpan.FromSeconds(3),//健康检查时间间隔，或者称为心跳 间隔
                    HTTP = $"{appUrl.Scheme}://{appUrl.Host}:{appUrl.Port}/health",//健康检查地址 
                    Timeout = TimeSpan.FromSeconds(15)   //超时时间
                }
            };

            client.Agent.ServiceRegister(consulServiceRegistration).Wait();
            appLifetime.ApplicationStopping.Register(() => { client.Agent.ServiceDeregister(consulServiceRegistration.ID).Wait(); });
        }
    }
}
