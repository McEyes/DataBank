using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.EmailTemplate.Dtos;

namespace ITPortal.Flow.Application.EmailTemplate
{
    public interface IEmailTemplateService : IBaseService<EmailTemplateEntity, EmailTemplateDto, Guid>
    {
    }
}
