using Furion;

using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.LightElasticSearch.Providers;

using Microsoft.Extensions.DependencyInjection;

namespace ITPortal.Core.Extensions
{
    public static class ElasticSearchExtensions
    {
        public static IServiceCollection AddElasticSearch(this IServiceCollection services, Action<ElasticSearchOptions> configure = null)
        {
            var options = new ElasticSearchOptions();
            services.Configure<ElasticSearchOptions>(option =>
            {
                if (configure != null)
                {
                    configure?.Invoke(option);
                }
                else
                {
                    var config = App.GetConfig<ElasticSearchOptions>("Elastic");
                    option.Url = config.Url;
                    option.API_KEY = config.API_KEY;
                }
                options = option;
            });
            //services.AddStackExchangeRedisCache(option =>
            //{
            //    var config = App.GetConfig<RedisCacheOptions>("Redis");
            //    option.Configuration = config.Configuration;
            //    option.InstanceName = config.InstanceName;
            //});
            services.Add(ServiceDescriptor.Singleton<IElasticsearchProvider, ElasticsearchProvider>());
            return services;
        }
    }
}
