namespace jb.home.Service.Application.Contracts.FlowDataGrantApplys
{
    public interface IFlowDataGrantApplyAppService
    {
        Task<ResponseResult> InitFlowAsync(FlowDataGrantModel input);
        Task<ResponseResult<ApplyFlowInfoInitDto>> CreateFlowDataTableGrantApplyAsync(FlowDataGrantModel input);

        Task<ResponseResultEXDto<FlowDataTableGrantApplyDto>> GetDetailAsync(Guid id);

        //Task<ResponseResult<IEnumerable<FlowDataTableGrantApplyDto>>> GetListAsync();

        //Task<ResponseResult<PagedResultDto<FlowDataTableGrantApplyDto>>> GetPagedListAsync(string title, int pageIndex = 1, int pageSize = 20);

        Task<ResponseResult> SendApprove(string initUrl, ApplyFlowApproveRecordInput record);
        Task<ResponseResult> UpdateFlowDataTableGrantApplyAsync(CreateFlowDataTableGrantApplyInputDto input);

        Task<ResponseResult> DeleteFlowDataTableGrantApplyAsync(string id);
        Task<ResponseResult> CancelFlowDataTableGrantApplyAsync(string formNo);
        Task<ResponseResult<DataGrantAuthBackOutput>> ApproveCallBack(DataGrantAuthBackInput input);
    }
}
