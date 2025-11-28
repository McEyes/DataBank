using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.EmailSendRecord.Dtos;

namespace ITPortal.Flow.Application.EmailSendRecord
{
    public class EmailSendRecordService : BaseService<EmailSendRecordEntity, EmailSendRecordDto, Guid>, IEmailSendRecordService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public EmailSendRecordService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<EmailSendRecordEntity> BuildFilterQuery(EmailSendRecordDto filter)
        {
            return CurrentDb.Queryable<EmailSendRecordEntity>()
                .WhereIF(filter.Id!=Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.EmailSubject), f => SqlFunc.ToLower(f.EmailSubject).Contains(filter.EmailSubject.ToLower()))
                .WhereIF(filter.FlowInstId.HasValue, f => f.FlowInstId.Equals(filter.FlowInstId))
                .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                .OrderByDescending(f => f.CreateTime);
        }
    }
}
