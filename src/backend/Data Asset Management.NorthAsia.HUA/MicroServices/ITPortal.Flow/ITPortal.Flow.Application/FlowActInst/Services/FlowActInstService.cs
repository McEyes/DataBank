using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowActInst.Dtos;

namespace ITPortal.Flow.Application.FlowActInst
{
    public class FlowActInstService : BaseService<FlowActInstEntity, FlowActInstDto, Guid>, IFlowActInstService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public FlowActInstService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<FlowActInstEntity> BuildFilterQuery(FlowActInstDto filter)
        {
            return CurrentDb.Queryable<FlowActInstEntity>()
                .WhereIF(filter.Id!=Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ActName), f => f.ActName == filter.ActName)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ActTitle), f => f.ActTitle.Contains( filter.ActTitle))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ActName), f => f.ActName == filter.ActName)
                .WhereIF(filter.FlowInstId.HasValue, f => f.FlowInstId.Equals(filter.FlowInstId))
                .WhereIF(filter.ActStatus.HasValue, f => f.ActStatus == filter.ActStatus)
                .OrderByDescending(f => f.ActStep);
        }
    }
}
