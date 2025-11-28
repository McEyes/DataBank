using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowAttachments.Dtos;
using ITPortal.Flow.Application.FlowAttachments.Services;

namespace ITPortal.Flow.Application.FlowAttachment.Services
{
    public class FlowAttachmentService : BaseService<FlowAttachmentEntity, FlowAttachmentDto, Guid>, IFlowAttachmentService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public FlowAttachmentService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<FlowAttachmentEntity> BuildFilterQuery(FlowAttachmentDto filter)
        {
            return CurrentDb.Queryable<FlowAttachmentEntity>()
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.FileName), f => SqlFunc.ToLower(f.FileName).Contains(filter.FileName.ToLower()))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.CreateBy), f => SqlFunc.ToLower(f.CreateBy) == filter.CreateBy.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.FileType), f => SqlFunc.ToLower(f.FileType) == filter.FileType.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.FlowTempName), f => SqlFunc.ToLower(f.FlowTempName) == filter.FlowTempName.ToLower())
                .WhereIF(filter.FlowInstId.HasValue, f => f.FlowInstId.Equals(filter.FlowInstId))
                .WhereIF(filter.FlowTempId.HasValue, f => f.FlowTempId.Equals(filter.FlowTempId))
                .OrderBy(f => f.CreateTime);
        }
    }
}
