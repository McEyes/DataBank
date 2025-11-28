using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Core;
using DataAssetManager.DataTableServer.Application;

using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.EvenBus;

using Microsoft.Extensions.Logging;

namespace DataAssetManager.DataApiServer.Application.EventSub
{
    public class DiffLogSubscriber : IEventSubscriber, ISingleton
    {
        private readonly ILogger<LogEventSubscriber> _logger;
        private readonly IDataChangeRecordService _logService;
        public DiffLogSubscriber(ILogger<LogEventSubscriber> logger
            , IDataChangeRecordService logService)
        {
            _logService = logService;
            _logger = logger;
        }


        [EventSubscribe(DataAssetManagerConst.DataChangeRecordEvent)]
        public async Task RecordLog(EventHandlerExecutingContext context)
        {
            try
            {
                var list = context.GetSourcePayload<List<DataChangeRecordEntity>>();
                var result = await _logService.CurrentDb.Insertable(list).ExecuteCommandAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"RecordLog异常：{ex.Message},\r\n{ex.StackTrace}");
            }
            _logger.LogInformation($"API Log Record Event:{context.Source.EventId}");
        }
    }
}
