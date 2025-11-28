using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;
using ITPortal.Flow.Application.FlowInst.Dtos;
using ITPortal.Flow.Core;

namespace ITPortal.Flow.Application.FlowInst.Services
{
    public interface IFlowInstService : IBaseService<FlowInstEntity, FlowInstDto, Guid>
    {
        Task<FlowInstEntity> GetInfo(Guid flowId);
        Task<ITPortal.Core.Services.IResult> StartFlowInst(StartFlowDto flowTempDto, FlowAction action = FlowAction.Submit);
        Task<ITPortal.Core.Services.IResult> Approval(FlowAuditDto info);
        Task<ITPortal.Core.Services.IResult> Reject(FlowAuditDto info);
        Task<ITPortal.Core.Services.IResult> RejectStart(FlowAuditDto info);
        Task<ITPortal.Core.Services.IResult> Submit(FlowAuditDto info, FlowAction action);
        Task<ITPortal.Core.Services.IResult> RejectEnd(FlowAuditDto info);
        Task<ITPortal.Core.Services.IResult> Transfer(FlowTransferAuditDto info);
        Task<ITPortal.Core.Services.IResult> GotToAct(FlowGotToActAuditDto info);
        Task<PageResult<FlowEntity>> PageMyRequests(FlowInstDto filter);
        Task<int> Delete(string flowNo);
        Task<PageResult<FlowEntity>> WorkListPageQuery(FlowInstDto filter);
        Task<FlowInstInfo> GetFormFlowInfo(Guid flowId);
        Task<FlowInstInfo> GetFormFlowInfoByTaskId(Guid taskId);
    }
}
