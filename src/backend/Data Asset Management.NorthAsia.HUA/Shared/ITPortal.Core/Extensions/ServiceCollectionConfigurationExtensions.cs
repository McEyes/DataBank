using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Extensions
{

    public static class ServiceCollectionConfigurationExtensions
    {
        public static IServiceCollection ReplaceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Replace(ServiceDescriptor.Singleton<IConfiguration>(configuration));
        }

        public static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            var hostBuilderContext = services.GetSingletonInstanceOrNull<HostBuilderContext>();
            if (hostBuilderContext?.Configuration != null)
            {
                return hostBuilderContext.Configuration as IConfigurationRoot;
            }

            return services.GetSingletonInstance<IConfiguration>();
        }
    }

}
