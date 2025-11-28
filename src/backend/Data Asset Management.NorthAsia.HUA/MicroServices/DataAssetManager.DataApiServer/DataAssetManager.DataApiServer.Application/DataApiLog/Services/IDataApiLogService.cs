using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.Services;

namespace DataAssetManager.DataTableServer.Application
{
    public interface IDataApiLogService : IBaseService<DataApiLogEntity, DataApiLogDto, string>
    {
        Task<List<DayVisitedDto>> GetApiDailyStats(DateTime startDate);
        Task<PageResult<DataApiLogView>> LogViewQuery(DataApiLogQuery filter);
        Task SendLogEvent(HttpContext context, RouteInfo routInfo, int times, string msg, int status = 0, long callerSize = 0);
    }
}
