using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataTableServer.Application;

using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.EvenBus;

using Microsoft.Extensions.Logging;

namespace DataAssetManager.DataApiServer.Application.EventSub
{
    public class LogEventSubscriber : IEventSubscriber, ISingleton
    {
        private readonly ILogger<LogEventSubscriber> _logger;
        private readonly IDataApiLogService _logService;
        public LogEventSubscriber(ILogger<LogEventSubscriber> logger
            , IDataTableService dataTableService
            , IDataApiLogService logService)
        {
            _logService = logService;
            _logger = logger;
        }


        [EventSubscribe(DataAssetManagerConst.LogRecordEvent)]
        public async Task RecordLog(EventHandlerExecutingContext context)
        {
            try
            {
                await _logService.Create(context.GetSourcePayload<DataApiLogEntity>());
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"RecordLog异常：{ex.Message},\r\n{ex.StackTrace}");
            }
            _logger.LogInformation($"API Log Record Event:{context.Source.EventId}");
        }
    }
}
