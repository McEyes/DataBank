using DataAssetManager.DataTableServer.Application;

using Elastic.Clients.Elasticsearch;

using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.EvenBus;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.LightElasticSearch.Providers;

using Microsoft.Extensions.Logging;

namespace DataAssetManager.DataApiServer.Application.EventSub
{
    public class TrackLogEventSubscriber : IEventSubscriber, ISingleton
    {
        private readonly ILogger<TrackLogEventSubscriber> _logger;
        private readonly ITrackLogService _logService;
        protected LightElasticsearchService _elasticSearchService { get; private set; }
        public TrackLogEventSubscriber(ILogger<TrackLogEventSubscriber> logger
            , IDataTableService dataTableService
            , ITrackLogService logService)
        {
            _logger = logger;
            _logService = logService;
            _elasticSearchService = App.GetService<LightElasticsearchService>();
        }

       //new ChannelEventSource(DataAssetManagerConst.DataTable_UserHashKey)
        [EventSubscribe(DataAssetManagerConst.TrackLogRecordEvent)]
        public async Task RecordLog(EventHandlerExecutingContext context)
        {
            try
            {
                await _logService.Create(context.GetSourcePayload<ApiTrackLogInfo>());
                if (context.Source is ElasticEventSource)
                {
                    var data = context.Source as ElasticEventSource;
                    var elasResult = await _elasticSearchService.InsertAsync(data.Payload, data.ModelType);
                    if (!elasResult.IsSuccess())
                    {
                        _logger.LogError($"Insert Elastic Index Error:{elasResult.DebugInformation}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"RecordLog异常：{ex.Message},\r\n{ex.StackTrace}");
            }
            _logger.LogInformation($"Track Log Record Event :{context.Source.EventId}");
        }

    }
}
