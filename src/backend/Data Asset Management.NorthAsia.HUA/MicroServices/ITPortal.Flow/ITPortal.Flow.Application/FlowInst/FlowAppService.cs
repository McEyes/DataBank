using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;
using ITPortal.Flow.Application.FlowInst;
using ITPortal.Flow.Application.FlowInst.Dtos;
using ITPortal.Flow.Application.FlowInst.Services;
using ITPortal.Flow.Application.FlowTodo.Dtos;
using ITPortal.Flow.Application.FlowTodo.Services;
using ITPortal.Flow.Core;

using System;

namespace ITPortal.Flow.Application.FlowInst
{
    /// <summary>
    /// 流程节点实例
    /// </summary>
    [AppAuthorize]
    [Route("api/Flow/", Name = "流程服务中心")]
    [ApiDescriptionSettings(GroupName = "流程服务中心", Area = "流程服务中心", Order = 1, Version = "1.0")]
    public class FlowAppService : IDynamicApiController
    {
        private readonly IFlowInstService _FlowInstService;
        private readonly IFlowTodoService _FlowTodoService;

        public FlowAppService(IFlowInstService dataApiService, IFlowTodoService todoApiService)
        {
            _FlowInstService = dataApiService;
            _FlowTodoService = todoApiService;
        }

        /// <summary>
        /// 检查config参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public dynamic GetConfig(string key= "RemoteApi:AppHostUrl")
        {
            return App.GetConfig<string>(key);
        }


        /// <summary>
        /// 获取未处理的待办清单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("todo/Self")]
        public async Task<PageResult<FlowTodoEntity>> PageSelfTodo(FlowTodoDto filter)
        {
            return await _FlowTodoService.PageSelfTodo(filter);
        }


        /// <summary>
        /// 获取已处理的待办清单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("todo/Deal")]
        public async Task<PageResult<FlowTodoEntity>> PageDealTodo(FlowTodoDto filter)
        {
            return await _FlowTodoService.PageDealTodo(filter);
        }

        /// <summary>
        /// 获取自己的待办清单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("todo/Self/Page")]
        public async Task<PageResult<FlowTodoEntity>> PageSelfTodoQuery(FlowTodoDto filter)
        {
            filter.OwnerID = _FlowTodoService.CurrentUser.UserId.ToLower();
            return await _FlowTodoService.PageQuery(filter);
        }


        /// <summary>
        /// 获取待办清单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("todo/Page")]
        public async Task<PageResult<FlowTodoEntity>> PageTodoQuery(FlowTodoDto filter)
        {
            if (!_FlowTodoService.CurrentUser.IsDataAssetManager)
            {
                filter.OwnerID = _FlowTodoService.CurrentUser.UserId.ToLower();
            }
            return await _FlowTodoService.PageQuery(filter);
        }


        /// <summary>
        /// 申请清单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("MyRequests")]
        public async Task<PageResult<FlowEntity>> PageMyRequests(FlowInstDto filter)
        {
            return await _FlowInstService.PageMyRequests(filter);
        }


        /// <summary>
        /// 获取所有发起流程
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("Page")]
        public async Task<PageResult<FlowEntity>> PageQuery(FlowInstDto filter)
        {
            return await _FlowInstService.WorkListPageQuery(filter);
        }



        /// <summary>
        /// 获取流程实例信息
        /// 包含审批节点信息
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        [HttpPost("FlowInst/{flowId}")]
        public async Task<FlowInstEntity> GetFlowInstInfo(Guid flowId)
        {
            return await _FlowInstService.GetInfo(flowId);
        }


        /// <summary>
        /// 获取流程实例和表单信息
        /// 包含审批节点信息，审批记录，附件等
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        [HttpGet("FlowForm/{flowId}")]
        public async Task<FlowInstInfo> GetFormFlowInfo(Guid flowId)
        {
            return await _FlowInstService.GetFormFlowInfo(flowId);
        }

        /// <summary>
        /// 获取流程实例和表单信息
        /// 包含审批节点信息，审批记录，附件等
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpGet("FlowForm/todo/{taskId}")]
        public async Task<FlowInstInfo> GetFormFlowInfoByTodoId(Guid taskId)
        {
            return await _FlowInstService.GetFormFlowInfoByTaskId(taskId);
        }


        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="flowData"></param>
        /// <returns></returns>
        [HttpPost("Start")]
        public async Task<ITPortal.Core.Services.IResult> StartFlowInst(StartFlowDto flowData)
        {
            return await _FlowInstService.StartFlowInst(flowData);
        }

        /// <summary>
        /// 保存草稿
        /// </summary>
        /// <param name="flowData"></param>
        /// <returns></returns>
        [HttpPost("SaveDraft")]
        public async Task<ITPortal.Core.Services.IResult> SaveDraft(StartFlowDto flowData)
        {
            return await _FlowInstService.StartFlowInst(flowData, FlowAction.Draft);
        }

        /// <summary>
        /// 流程审批
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<ITPortal.Core.Services.IResult> Approval(FlowAuditDto info)
        {
            return await _FlowInstService.Approval(info);
        }
        /// <summary>
        /// 流程驳回
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<ITPortal.Core.Services.IResult> Reject(FlowAuditDto info)
        {
            return await _FlowInstService.Reject(info);
        }
        /// <summary>
        /// 流程驳回到开始节点
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("RejectStart")]
        public async Task<ITPortal.Core.Services.IResult> RejectStart(FlowAuditDto info)
        {
            return await _FlowInstService.RejectStart(info);
        }
        /// <summary>
        /// 拒绝结束流程
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("RejectEnd")]
        public async Task<ITPortal.Core.Services.IResult> RejectEnd(FlowAuditDto info)
        {
            return await _FlowInstService.RejectEnd(info);
        }

        /// <summary>
        /// 拒绝结束流程
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("cancel")]
        public async Task<ITPortal.Core.Services.IResult> Cancel(FlowAuditDto info)
        {
            return await _FlowInstService.Submit(info, FlowAction.Cancel);
        }
        /// <summary>
        /// 转办
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("Transfer")]
        public async Task<ITPortal.Core.Services.IResult> Transfer(FlowTransferAuditDto info)
        {
            return await _FlowInstService.Transfer(info);
        }

        /// <summary>
        /// 转办待办，
        /// 只支持待办id转办
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("TransferTodo")]
        public async Task<ITPortal.Core.Services.IResult> TransferTodo(FlowTransferAuditDto info)
        {
            if (!info.Id.HasValue) return Result.Fail("参数待办id不能为空！");
            return await _FlowInstService.Transfer(info);
        }


        /// <summary>
        /// 跳转到指定节点
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("GotToAct")]
        public async Task<ITPortal.Core.Services.IResult> GotToAct(FlowGotToActAuditDto info)
        {
            return await _FlowInstService.GotToAct(info);
        }


        /// <summary>
        /// 流程流转审批，驳回等操作
        /// </summary>
        /// <param name="info"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<ITPortal.Core.Services.IResult> Submit(FlowAuditDto info, FlowAction action)
        {
            return await _FlowInstService.Submit(info, action);
        }

        /// <summary>
        /// 流程流转审批，驳回等操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Delete/{id}")]
        [HttpDelete("Delete/{id}")]
        [HttpDelete("Delete")]
        public async Task<bool> Delete(Guid id)
        {
            return await _FlowInstService.Delete(id);
        }

        /// <summary>
        /// 流程流转审批，驳回等操作
        /// </summary>
        /// <param name="flowNo"></param>
        /// <returns></returns>
        [HttpPost("DeleteByNo/{flowNo}")]
        [HttpDelete("DeleteByNo")]
        [HttpPost("DeleteByNo")]
        public async Task<int> DeleteByNo(string flowNo)
        {
            return await _FlowInstService.Delete(flowNo);
        }

    }
}
