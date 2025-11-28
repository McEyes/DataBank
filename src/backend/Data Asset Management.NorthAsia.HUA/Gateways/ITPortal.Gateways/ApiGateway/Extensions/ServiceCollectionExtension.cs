using ApiGateway.Models;

using Consul;

using Microsoft.Extensions.Configuration;
using System.Net;

namespace ApiGateway.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCors2(this IServiceCollection builder, ConfigurationManager configuration)
        {
            var appSetting = configuration.GetSection("AppSetting").Get<AppSetting>();
            builder.AddCors(options =>
            {
                options.AddPolicy("Default", builder =>
                {
                    builder
                        .WithOrigins(
                            appSetting.CorsOrigins
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .ToArray()
                        )
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return builder;
        }

        public static IServiceCollection AddConsulConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var consulHost = configuration["GlobalConfiguration:ServiceDiscoveryProvider:Host"]; 
                var consulPort = 8500;
                if (int.TryParse(configuration["GlobalConfiguration:ServiceDiscoveryProvider:Port"], out int port))
                    consulPort = port;
                var token = configuration["GlobalConfiguration:ServiceDiscoveryProvider:Token"];
                consulConfig.Address = new Uri($"http://{consulHost}:{consulPort}");
                consulConfig.Token = token;
            }));
            services.AddHealthChecks();
            return services;
        }

    }
}
