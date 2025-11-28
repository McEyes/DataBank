using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataTableServer.Application
{
    public interface IDataAuthApplyService : IBaseService<DataAuthApplyEntity, DataAuthApplyDto, Guid>
    {
        /// <summary>
        /// 申请流程
        /// </summary>
        /// <param name="authApplyDto"></param>
        /// <returns></returns>
        Task<ITPortal.Core.Services.IResult> ApplyAuth(DataAuthApplyInfo authApplyDto);
        Task<Result<ITPortal.Core.ProxyApi.Flow.Dto.FlowInstEntity>> InitApplyFlow(DataAuthApplyEntity entity, List<ApproveDto> appList);

        ///// <summary>
        ///// 检查权限
        ///// </summary>
        ///// <param name="authApplyDto"></param>
        ///// <returns></returns>
        //Task<ITPortal.Core.Services.IResult> CheckAuth(DataAuthApplyInfo authApplyDto);
        /// <summary>
        /// 更新流程信息
        /// </summary>
        /// <param name="authResultDto"></param>
        /// <returns></returns>
        Task<string> AuthBack(Result<FlowBackDataEntity> authResultDto);
        Task<Result<FlowInstEntity>> StartFlow(DataAuthApplyEntity entity);
    }
}
