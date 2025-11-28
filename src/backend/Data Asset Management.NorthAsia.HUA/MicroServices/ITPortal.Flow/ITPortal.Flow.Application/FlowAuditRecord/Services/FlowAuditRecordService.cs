using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;

namespace ITPortal.Flow.Application.FlowAuditRecord.Services
{
    public class FlowAuditRecordService : BaseService<FlowAuditRecordEntity, FlowAuditRecordDto, Guid>, IFlowAuditRecordService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public FlowAuditRecordService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<FlowAuditRecordEntity> BuildFilterQuery(FlowAuditRecordDto filter)
        {
            return CurrentDb.Queryable<FlowAuditRecordEntity>()
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ActName), f => SqlFunc.ToLower(f.ActName) == filter.ActName.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Approver), f => SqlFunc.ToLower(f.Approver) == filter.Approver.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ApproverName), f => SqlFunc.ToLower(f.ApproverName) == filter.ApproverName.ToLower())
                .WhereIF(filter.FlowInstId.HasValue, f => f.FlowInstId.Equals(filter.FlowInstId))
                .WhereIF(filter.ActId.HasValue, f => f.ActId.Equals(filter.ActId))
                .OrderByDescending(f => f.ActStep);
        }
    }
}
