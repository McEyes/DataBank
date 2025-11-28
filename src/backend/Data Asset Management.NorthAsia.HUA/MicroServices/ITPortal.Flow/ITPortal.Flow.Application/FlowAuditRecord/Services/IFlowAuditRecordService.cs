using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;

namespace ITPortal.Flow.Application.FlowAuditRecord.Services
{
    public interface IFlowAuditRecordService : IBaseService<FlowAuditRecordEntity, FlowAuditRecordDto, Guid>
    {
    }
}
