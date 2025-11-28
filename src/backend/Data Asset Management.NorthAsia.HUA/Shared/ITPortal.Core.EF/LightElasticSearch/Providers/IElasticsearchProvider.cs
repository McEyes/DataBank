using Elastic.Clients.Elasticsearch;

using Furion.DependencyInjection;

namespace ITPortal.Core.LightElasticSearch.Providers
{
    public interface IElasticsearchProvider : ISingleton
    {
        ElasticsearchClient GetElasticClient();
    }
}