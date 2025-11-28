using DataAssetManager.DataApiServer.Application.FlowDataSourceApplys.Dtos;

using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application.FlowDataSourceApply.Services
{
    public interface IFlowDataSourceApplyService : IBaseService<FlowDataSourceApplyEntity, FlowDataSourceApplyDto, Guid>
    {
        Task<ITPortal.Core.Services.IResult> ApplyAuth(FlowSourceApply applyData);
        Task<IResult<string>> CallBack(Result<FlowBackDataEntity> result);
        Task<Result<FlowInstEntity>> StartFlow(FlowDataSourceApplyEntity entity);
    }
}
