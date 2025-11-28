using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataTableServer.Application
{
    public interface IDataAuthAuditRecordService : IBaseService<DataAuthAuditRecordEntity, DataAuthAuditRecordDto, string>
    {
        Task<ITPortal.Core.Services.IResult> ApplyAuth(DataAuthApply authApplyDto);
        Task<ITPortal.Core.Services.IResult> CheckAuth(DataAuthApply authApplyDto);
        //Task<Result<JabusEmployeeInfo>> GetEmployeeAsync(string ntid);
        //Task<Result<List<JabusUserInfo>>> GetUserInfo();
        Task<ITPortal.Core.Services.IResult<string>> UpdateAuth(AuthResultDto authResultDto);
    }
}
