using Furion.EventBus;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.EmailTemplate.Dtos;

namespace ITPortal.Flow.Application.EmailTemplate
{
    public class EmailTemplateService : BaseService<EmailTemplateEntity, EmailTemplateDto, Guid>, IEmailTemplateService, ITransient
    {

        private readonly IEventPublisher _eventPublisher;
        public EmailTemplateService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<EmailTemplateEntity> BuildFilterQuery(EmailTemplateDto filter)
        {
            return CurrentDb.Queryable<EmailTemplateEntity>()
                .WhereIF(filter.Id != Guid.Empty, f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.EmailName), f => SqlFunc.ToLower(f.EmailName )== filter.EmailName.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.EmailTitle), f => SqlFunc.ToLower(f.EmailTitle).Contains(filter.EmailTitle.ToLower()))
                .WhereIF(filter.IsEnabled.HasValue, f => f.IsEnabled.Equals(filter.IsEnabled))
                .OrderByDescending(f => f.UpdateTime);
        }
    }
}
