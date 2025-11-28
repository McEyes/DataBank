using Elastic.Clients.Elasticsearch;

using Furion;
using Furion.DependencyInjection;
using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.LightElasticSearch.Providers;

using Microsoft.Extensions.Logging;

namespace ITPortal.Core.EvenBus.EventSub
{
    public class ElasticEventSubscriber : IEventSubscriber, ISingleton
    {
        private readonly ILogger<ElasticEventSubscriber> _logger;
        protected LightElasticsearchService _elasticSearchService { get; private set; }
        public ElasticEventSubscriber(ILogger<ElasticEventSubscriber> logger)
        {
            _logger = logger;
            _elasticSearchService = App.GetService<LightElasticsearchService>();
        }

        [EventSubscribe(DataAssetManagerConst.ElasticRecordEvent)]
        public async Task RecordLog(EventHandlerExecutingContext context)
        {
            try
            {
                if (context.Source is ElasticEventSource)
                {
                    var data = context.Source as ElasticEventSource;
                    var elasResult = await _elasticSearchService.InsertAsync(data.Payload, data.IndexType);
                    if (!elasResult.IsSuccess())
                    {
                        _logger.LogError($"Insert Elastic Index Error:{elasResult.DebugInformation}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: $"RecordLog异常：{ex.Message},\r\n{ex.StackTrace}");
            }
        }

    }
}
