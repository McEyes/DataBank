using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTempAct.Dtos;

namespace ITPortal.Flow.Application.FlowTempAct.Services
{
    public class FlowTempActService : BaseService<FlowTempActEntity, FlowTempActDto, Guid>, IFlowTempActService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public FlowTempActService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<FlowTempActEntity> BuildFilterQuery(FlowTempActDto filter)
        {
            return CurrentDb.Queryable<FlowTempActEntity>()
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ActTitle), f => SqlFunc.ToLower(f.ActTitle).Contains(filter.ActTitle.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ActName), f => SqlFunc.ToLower(f.ActName) == filter.ActName.ToLower())
                .WhereIF(filter.FlowTempID.HasValue, f => f.FlowTempID.Equals(filter.FlowTempID))
                .OrderByDescending(f => f.ActStep);
        }
    }
}
