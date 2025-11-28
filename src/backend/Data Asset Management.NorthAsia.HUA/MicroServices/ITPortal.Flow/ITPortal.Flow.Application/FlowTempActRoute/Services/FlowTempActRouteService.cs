using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTempActRoute.Dtos;

namespace ITPortal.Flow.Application.FlowTempActRoute.Services
{
    public class FlowTempActRouteService : BaseService<FlowTempActRouteEntity, FlowTempActRouteDto, Guid>, IFlowTempActRouteService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public FlowTempActRouteService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<FlowTempActRouteEntity> BuildFilterQuery(FlowTempActRouteDto filter)
        {
            return CurrentDb.Queryable<FlowTempActRouteEntity>()
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id);
        }
    }
}
