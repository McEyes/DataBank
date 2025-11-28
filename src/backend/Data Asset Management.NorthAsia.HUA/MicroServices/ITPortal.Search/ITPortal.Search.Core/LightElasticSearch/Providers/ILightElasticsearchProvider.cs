using Furion.DependencyInjection;

using Nest;

namespace ITPortal.Search.Core.LightElasticSearch.Providers
{
    public interface ILightElasticsearchProvider : ISingleton
    {
        IElasticClient GetElasticClient();
    }
}