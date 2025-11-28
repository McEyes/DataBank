using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.Services;

namespace DataAssetManager.DataTableServer.Application
{
    public interface ITrackLogService : IBaseService<ApiTrackLogInfo, TrackLogDto, Guid>
    {
        //Task SendLogEvent(HttpContext context, TrackLogDto apiLogDto);
    }
}
