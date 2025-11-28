using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;
using DataAssetManager.DataTableServer.Application;

using Elastic.Clients.Elasticsearch;

using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.LightElasticSearch.Providers;

using Microsoft.Extensions.Logging;

namespace DataAssetManager.DataApiServer.Application.EventSub
{
    public class SearchDocumentEventSubscriber : IEventSubscriber, ISingleton
    {
        private readonly ILogger<SearchDocumentEventSubscriber> _logger;
        private readonly IOpenSearchService _openSearchService;
        private readonly IDataCatalogService _dataCatalogService;
        private readonly IDataSourceService _dataSourceService;
        private readonly IDataTableService _dataTableService;
        private readonly IDataColumnService _dataColumnService;
        private readonly IDataApiService _dataApiService;
        public SearchDocumentEventSubscriber(ILogger<SearchDocumentEventSubscriber> logger,
            IDataSourceService dataSourceService,
            IDataTableService dataTableService,
            IDataColumnService dataColumnService,
            IDataApiService dataApiService, IDataCatalogService dataCatalogService, IOpenSearchService openSearchService)
        {
            _logger = logger;
            _openSearchService = openSearchService;// App.GetService<LightElasticsearchService>();
            _dataCatalogService = dataCatalogService;
            _dataSourceService = dataSourceService;
            _dataTableService = dataTableService;
            _dataColumnService = dataColumnService;
            _dataApiService = dataApiService;
        }

        [EventSubscribe(DataAssetManagerConst.ElasticCatalogEvent)]
        public async Task RecordLog(EventHandlerExecutingContext context)
        {
            try
            {
                if (context.Source is ElasticEventSource)
                {
                    var data = context.Source as ElasticEventSource;
                    var elasResult = await _openSearchService.CreateTopicDocument(data.Payload, data.IndexType);
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
