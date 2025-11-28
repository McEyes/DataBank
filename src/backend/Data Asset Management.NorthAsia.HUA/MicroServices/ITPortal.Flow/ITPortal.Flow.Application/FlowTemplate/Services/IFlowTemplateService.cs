using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTemplate.Dtos;

namespace ITPortal.Flow.Application.FlowTemplate.Services
{
    public interface IFlowTemplateService : IBaseService<FlowTemplateEntity, FlowTemplateDto, Guid>
    {
        Task<Result<string>> CheckTempRoute(Guid tempId);
        Task<FlowTemplateDto> GetTempInfo(string tempName);
        Task<FlowTemplateDto> GetTempInfo(Guid tempId);
    }
}
