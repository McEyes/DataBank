using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataTableServer.Application;

using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application
{
    public class TrackLogService : BaseService<ApiTrackLogInfo, TrackLogDto, Guid>, ITrackLogService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public TrackLogService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, false)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<ApiTrackLogInfo> BuildFilterQuery(TrackLogDto filter)
        {
            return CurrentDb.Queryable<ApiTrackLogInfo>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ApiAction), f => f.ApiAction == filter.ApiAction)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Path), f => SqlFunc.ToLower(f.Path).Contains(filter.Path.ToLower()))
                .WhereIF(filter.StatusCode.HasValue, f => f.StatusCode.Equals(filter.StatusCode));
        }

        //public async Task SendLogEvent(HttpContext context, TrackLogDto apiLogDto)
        //{
        //    await _eventPublisher.PublishAsync(DataAssetManagerConst.TrackLogRecordEvent, apiLogDto);
        //}

    }
}
