using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jb.home.Service.Application.Contracts.ApplyFlowAttacchments
{
    public interface IApplyFlowAttacchmentAppService
    {
        Task<ResponseResult> CreateApplyFlowAttacchmentAsync(CreateApplyFlowAttacchmentInputDto input);

        Task<ResponseResult<ApplyFlowAttacchmentDto>> GetDetailAsync(Guid id);

        Task<ResponseResult<IEnumerable<ApplyFlowAttacchmentDto>>> GetListAsync();

        Task<ResponseResult<PagedResultDto<ApplyFlowAttacchmentDto>>> GetPagedListAsync(string title, int pageIndex = 1, int pageSize = 20);


        //Task<ResponseResult> AddApplyFlowAttacchmentEvaluate(CreateApplyFlowAttacchmentEvaluateDto input);

        Task<ResponseResult> UpdateApplyFlowAttacchmentAsync(CreateApplyFlowAttacchmentInputDto input);

        //Task<ResponseResult> DeleteApplyFlowAttacchmentAsync(DeleteGuidInputDto input );

        //Task<ActionResult> ExportApplyFlowAttacchmentAsync(Guid id, int pageIndex = 1, int pageSize = 20);
    }
}
