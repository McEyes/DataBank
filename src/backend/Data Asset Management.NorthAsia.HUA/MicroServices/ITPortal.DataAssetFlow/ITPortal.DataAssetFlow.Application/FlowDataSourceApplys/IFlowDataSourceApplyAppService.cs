using jb.home.Application.Contracts.FlowDataSourceApplys;
using jb.home.Service.Application.Contracts.CommonDtos;
using jb.home.Service.Application.Contracts.FlowDataGrantApplys;
using jb.home.Service.Application.Contracts.FlowDataSourceApplys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jb.home.FlowDataSourceApplys
{
    public interface IFlowDataSourceApplyAppService
    {
        Task<ResponseResult<ApplyFlowInfoInitDto>> CreateFlowDataSourceApplyAsync(CreateFlowDataSourceApplyInputDto input);

        Task<ResponseResult> InitFlowAsync(CreateFlowDataSourceApplyInputDto input);
        Task<ResponseResult> SaveOtherFileAsync(CreateFlowDataSourceApplyInputDto input);

        Task<ResponseResultEXDto<FlowDataSourceApplyDto>> GetDetailAsync(Guid id);

        Task<ResponseResult<IEnumerable<FlowDataSourceApplyDto>>> GetListAsync();

        Task<ResponseResult<PagedResultDto<FlowDataSourceApplyDto>>> GetPagedListAsync(string title, int pageIndex = 1, int pageSize = 20);


        //Task<ResponseResult> AddFlowDataSourceApplyEvaluate(CreateFlowDataSourceApplyEvaluateDto input);

        Task<ResponseResult> UpdateFlowDataSourceApplyAsync(CreateFlowDataSourceApplyInputDto input);

        Task<ResponseResult> DeleteFlowDataSourceApplyAsync(DeleteGuidInputDto input);
        Task<ActionResult> GetTemplate();
        //Task<ActionResult> ExportFlowDataSourceApplyAsync(Guid id, int pageIndex = 1, int pageSize = 20);

        Task<ResponseResult> CancelFlowDataSourceApplyAsync(Guid id);
        Task<DataSourceAppCfgOutputDto> GetDataSourceApproverList();
    }
}
