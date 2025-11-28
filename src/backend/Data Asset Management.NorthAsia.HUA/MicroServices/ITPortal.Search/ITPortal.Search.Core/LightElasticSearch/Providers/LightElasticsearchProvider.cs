using Elasticsearch.Net;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Nest;

using System;
using System.Text;

namespace ITPortal.Search.Core.LightElasticSearch.Providers
{
    public class LightElasticsearchProvider : ILightElasticsearchProvider
    {
        private readonly IConfiguration _configuration;

        public LightElasticsearchProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            Logger = NullLogger<LightElasticsearchProvider>.Instance;
        }

        public ILogger<LightElasticsearchProvider> Logger { get; set; }

        public IElasticClient GetElasticClient()
        {
            var pool = new SingleNodeConnectionPool(new Uri(_configuration.GetValue<string>("ElasticSearch:Url")));
            var connectionSettings =
                new ConnectionSettings(pool);
            connectionSettings.EnableHttpCompression();
            connectionSettings.BasicAuthentication(_configuration.GetValue<string>("ElasticSearch:UserName"),
                _configuration.GetValue<string>("ElasticSearch:Password"));
#if DEBUG
            connectionSettings.DisableDirectStreaming();
            connectionSettings.OnRequestCompleted(details =>
            {
                if (details.Success)
                {
                    if (details.RequestBodyInBytes != null)
                    {

                        var reqJson = Encoding.UTF8.GetString(details.RequestBodyInBytes);
                        //Logger.LogInformation($"Request=>{reqJson}");
                    }
                    if (details.ResponseBodyInBytes != null)
                    {
                        var resJson = Encoding.UTF8.GetString(details.ResponseBodyInBytes);
                        //Logger.LogInformation($"Response=>{resJson}");
                    }
                }
                else
                {

                }
            });
#endif
            return new ElasticClient(connectionSettings);
        }
    }
}