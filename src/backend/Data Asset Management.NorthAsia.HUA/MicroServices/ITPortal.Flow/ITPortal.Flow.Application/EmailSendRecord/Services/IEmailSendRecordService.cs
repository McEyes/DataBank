using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.EmailSendRecord.Dtos;

namespace ITPortal.Flow.Application.EmailSendRecord
{
    public interface IEmailSendRecordService : IBaseService<EmailSendRecordEntity, EmailSendRecordDto, Guid>
    {
    }
}
