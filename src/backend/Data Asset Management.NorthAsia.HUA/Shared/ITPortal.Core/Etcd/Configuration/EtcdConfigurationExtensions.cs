using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Etcd.Configuration
{
    public static class EtcdConfigurationExtensions
    {
        public static IWebHostBuilder UseEtcdConfig(this IWebHostBuilder builder, bool reloadOnChange = false, Action<IConfigurationRoot> actionOnChange = null)
        {
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                hostingContext.Configuration = config.Build();
                var configRepository = new EtcdConfigurationRepository(hostingContext.Configuration.GetSection("EtcdConfig").Get<EtcdOptions>() ?? new EtcdOptions { });
                config.Add(new EtcdConfigurationProvider(configRepository, reloadOnChange, actionOnChange));
                hostingContext.Configuration = config.Build();
            });

            return builder;
        }

        public static IHostBuilder UseEtcdConfig(this IHostBuilder builder, bool reloadOnChange = false, Action<IConfigurationRoot> actionOnChange = null)
        {
            builder.ConfigureHostConfiguration(configBuider =>
            {
                configBuider.AddEnvironmentVariables(prefix: "ASPNETCORE_");

            });
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                hostingContext.Configuration = config.Build();
                var configRepository = new EtcdConfigurationRepository(hostingContext.Configuration.GetSection("EtcdConfig").Get<EtcdOptions>() ?? new EtcdOptions { });
                config.Add(new EtcdConfigurationProvider(configRepository, reloadOnChange, actionOnChange));
                hostingContext.Configuration = config.Build();
            });

            return builder;
        }

        //public static IConfigurationBuilder AddEtcd(this IConfigurationBuilder builder, IConfiguration etcdConfiguration, bool reloadOnChange = false, Action<IConfigurationRoot> actionOnChange = null)
        //{
        //    var configRepository = new EtcdConfigurationRepository(etcdConfiguration.Get<EtcdOptions>());
        //    return builder.Add(new EtcdConfigurationProvider(configRepository, reloadOnChange, actionOnChange));
        //}
    }
}
