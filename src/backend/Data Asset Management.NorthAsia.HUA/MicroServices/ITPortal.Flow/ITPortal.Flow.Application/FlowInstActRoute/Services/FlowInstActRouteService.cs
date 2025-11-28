using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowInstActRoute.Dtos;

namespace ITPortal.Flow.Application.FlowInstActRoute.Services
{
    public class FlowInstActRouteService : BaseService<FlowInstActRouteEntity, FlowInstActRouteDto, Guid>, IFlowInstActRouteService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public FlowInstActRouteService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<FlowInstActRouteEntity> BuildFilterQuery(FlowInstActRouteDto filter)
        {
            return CurrentDb.Queryable<FlowInstActRouteEntity>()
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id)
                .OrderByDescending(f => f.CreateTime);
        }
    }
}
