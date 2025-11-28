using System;
using System.Text;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

using Furion;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using StackExchange.Profiling.Internal;

namespace ITPortal.Core.LightElasticSearch.Providers
{
    public class ElasticsearchProvider : IElasticsearchProvider
    {
        private static ElasticsearchClient client;
        private static object _lock = new object();

        public ElasticsearchProvider()
        {
        }
        //private readonly ILogger<LightElasticsearchProvider> _logger;
        //public LightElasticsearchProvider(ILogger<LightElasticsearchProvider> logger)
        //{
        //    _logger = logger;
        //}

        public ILogger<ElasticsearchProvider> Logger { get; set; }

        public ElasticsearchClient GetElasticClient()
        {
            return GenElasticClient();
        }

        public static ElasticsearchClient GenElasticClient()
        {
            if (client == null)
            {
                lock (_lock)
                {
                    if (client == null)
                    {
                        var nodes = new List<Uri>();
                        var options = App.GetConfig<ElasticSearchOptions>("ElasticSearch") ?? new ElasticSearchOptions();
                        if (options.Url.IsNullOrWhiteSpace()) throw new NotSupportedException("请在config配置Elastic配置，格式为:Elastic:{Url:'url地址'}");
                        var urlList = options.Url?.Split(',');
                        foreach (var node in urlList)
                            nodes.Add(new Uri(node));
                        var pool = new StaticNodePool(nodes);

                        var settings = new ElasticsearchClientSettings(pool);
                        //.CertificateFingerprint("<FINGERPRINT>")
                        //.Authentication(new ApiKey("<API_KEY>"));
                        if (!options.API_KEY.IsNullOrWhiteSpace())
                            settings.Authentication(new ApiKey(options.API_KEY));
                        else if (!options.UserName.IsNullOrWhiteSpace())
                            settings.Authentication(new BasicAuthentication(options.UserName, options.Password));
                        client = new ElasticsearchClient(settings);
                        return client;
                    }
                }
            }
            return client;
        }
    }
}