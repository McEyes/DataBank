using Furion;
using Furion.DatabaseAccessor;
using Furion.HttpRemote;

using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.ProxyApi.Flow.Enums;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using System.ComponentModel.DataAnnotations.Schema;


namespace ITPortal.Core.ProxyApi
{

    /// <summary>
    /// data auth apply 流程接口代理
    /// </summary>
    public class FlowApplyProxyService : BaseProxyService
    {
        //private readonly IHttpRemoteService httpRemoteService;
        private readonly string BaseHostUrl;
        private string Authorization = "";
        public FlowApplyProxyService(IHttpRemoteService httpRemoteService) : base(httpRemoteService)
        {
            //this.httpRemoteService = httpRemoteService;
            BaseHostUrl = App.GetConfig<string>("RemoteApi:ITPortalFlowUrl");
        }

        //private string GetToken()
        //{
        //    var token = string.Empty;
        //    if (App.HttpContext != null && App.HttpContext.Request.Headers != null)
        //        token = App.HttpContext.Request.Headers["Authorization"];
        //    if (token.IsNotNullOrWhiteSpace()) token = token.Replace("Bearer ", "");
        //    return token;
        //}

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<FlowTemplateDto>?> GetFlowTempInfo(string tempName)
        {
            //GetAsAsync<Result<FlowTemplateDto>>
            var resultStr = await httpRemoteService.GetAsStringAsync($"{BaseHostUrl}/api/FlowTemplate/Info/name/{tempName}",
            builder => builder.AddAuthentication("Bearer", GetToken()));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                return resultStr.To<Result<FlowTemplateDto>>();
            }
            Result<FlowTemplateDto>? data = new Result<FlowTemplateDto>();
            data.AddMsg(result);
            return data;
        }


        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<FlowInstEntity>?> InitFlowAsync(StartFlowDto input)
        {
            var resultStr = await httpRemoteService.PostAsStringAsync($"{BaseHostUrl}/api/Flow/start",
            builder => builder.SetContent(input, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                return resultStr.To<Result<FlowInstEntity>>();
            }
            var data = new Result<FlowInstEntity>();
            data.AddMsg(result);
            return data;
        }

        /// <summary>
        /// 发起失败时删除流程
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<Result?> DeleteAsync(string applyFormNo) => await httpRemoteService.PostAsAsync<Result>($"{BaseHostUrl}/api/Flow/DeleteByNo?flowNo={applyFormNo}",
            builder => builder.SetContent(new object(), "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

        /// <summary>
        /// 发起失败时删除流程
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<Result?> DeleteAsync(Guid flowId) => await httpRemoteService.PostAsAsync<Result>($"{BaseHostUrl}/api/Flow/Delete/{flowId}",
            builder => builder.SetContent(new object(), "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

        ///// <summary>
        ///// 流程审批，通用审批
        ///// </summary>
        ///// <param name="applyFormNo"></param>
        ///// <returns></returns>
        //public async Task<Result?> ApproveAsync(ApplyFlowApproveRecordInput approveRecord) => await httpRemoteService.PostAsAsync<Result>($"{BaseHostUrl}/homeapi/api/home/FlowDataAuthApply/SendApprove",
        //    builder => builder.SetContent(new object(), "application/json;charset=utf-8"));

        /// <summary>
        /// 流程审批，通用审批
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<Result?> SendApproveAsync(FlowAuditDto approveRecord) => await httpRemoteService.PostAsAsync<Result<string>>($"{BaseHostUrl}/api/Flow/approval",
            builder => builder.SetContent(approveRecord, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));


        /// <summary>
        /// 流程审批，通用审批,
        /// job后台服务使用，手动传入token
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Result?> SendApproveAsync(FlowAuditDto approveRecord,string token) => await httpRemoteService.PostAsAsync<Result<string>>($"{BaseHostUrl}/api/Flow/approval",
            builder => builder.SetContent(approveRecord, "application/json;charset=utf-8").AddAuthentication("Bearer", token));

        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<Result?> SendRejectAsync(FlowAuditDto approveRecord) => await httpRemoteService.PostAsAsync<Result<string>>($"{BaseHostUrl}/api/Flow/reject",
            builder => builder.SetContent(approveRecord, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));
        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<Result?> SendRejectEndAsync(FlowAuditDto approveRecord) => await httpRemoteService.PostAsAsync<Result<string>>($"{BaseHostUrl}/api/Flow/rejectend",
            builder => builder.SetContent(approveRecord, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));
        /// <summary>
        /// 驳回到开始
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<Result?> SendRejectStartAsync(FlowAuditDto approveRecord) => await httpRemoteService.PostAsAsync<Result<string>>($"{BaseHostUrl}/api/Flow/rejectstart",
            builder => builder.SetContent(approveRecord, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

    }
}
namespace ITPortal.Core.ProxyApi.Flow.Dto
{
    public class StartFlowDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string FlowTempId { get; set; }
        public string FlowTempName { get; set; }
        public Guid FormId { get; set; }
        /// <summary>
        /// 为空时，根据template规则自动生成
        /// </summary>
        public string FormNo { get; set; }
        public dynamic FormData { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string Applicant { get; set; }

        /// <summary>
        /// 申请人名称
        /// </summary>
        public string ApplicantName { get; set; }


        /// <summary>
        /// 指定节点审批人
        /// </summary>
        public List<FlowActApprover> ActApprovers { get; set; } = new List<FlowActApprover>();
        /// <summary>
        /// 流程附件，统一管理
        /// </summary>
        public List<FlowAttachmentInfo> Attacchments { get; set; } = new List<FlowAttachmentInfo>();
    }
    public class FlowActApprover// : Entity<Guid>
    {

        //public int? ActStep { get; set; }
        public string ActName { get; set; }
        //public string ActTitle { get; set; }

        public List<StaffInfo> ActorParms { get; set; } = new List<StaffInfo>();

        public string ActorParmsName { get; set; }

    }

    public class FlowAttachmentInfo : EntityBase<Guid>// : AuditEntity<Guid>
    {
        //public Guid? FlowInstId { get; set; } = null;
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsNeed { get; set; } = false;
        //public Guid? FlowTempId { get; set; }
        //public string FlowTempName { get; set; }
        //public  DateTimeOffset? StartTime { get; set; }
        //public DateTimeOffset? EndTime { get; set; }
        //public string CreateBy { get; set; }
        public string CreatedByName { get; set; }
    }
    public class StaffInfo
    {
        /// <summary>
        /// Ntid
        /// </summary>
        public string Ntid { get; set; }
        /// <summary>
        /// 显示名称：
        /// </summary>
        public string Name { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class FlowAuditDto
    {
        /// <summary>
        /// 待办id，待办id和流程id二选一，不需要有一个
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// 流程id，待办id和流程id二选一，不需要有一个
        /// </summary>
        public Guid? FlowInstId { get; set; }
        //public int? ActStep { get; set; }
        //public string ActTitle { get; set; }
        //public string ActName { get; set; }
        //public string Approver { get; set; }
        //public string ApproverName { get; set; }
        /// <summary>
        /// 审批动作
        /// </summary>
        public string ActOperate { get; set; }
        /// <summary>
        /// 审批内容
        /// </summary>
        public string AuditContent { get; set; }
        ///// <summary>
        ///// 审批备注
        ///// </summary>
        //public string Remark { get; set; }
        //public string Transferor { get; set; }
        //public string TransferorName { get; set; }
        //附件
    }


    public class FlowTemplateDto : Entity<Guid>
    {
        /// <summary>
        /// 流程编号模板
        /// </summary>
        public string FlowNo { get; set; }
        public string FlowName { get; set; }

        public string FlowTitle { get; set; }
        public string FormTemplate { get; set; }

        public string TaskTitle { get; set; }
        public string FormUrl { get; set; }
        public string CallBackUrl { get; set; }
        public int? Sort { get; set; }
        public int? Status { get; set; }

        public string Description { get; set; }
        public int? NoticeType { get; set; }

        public string Ver { get; set; }
        public Guid? EmailTempID { get; set; }

        public List<FlowTempActDto> FlowActs { get; set; } = new List<FlowTempActDto>();
    }
    public class FlowTempActDto : Entity<Guid>
    {
        public Guid? FlowTempID { get; set; }
        public int? ActStep { get; set; }
        public string ActName { get; set; }
        public string ActTitle { get; set; }
        public string FormUrl { get; set; }

        public string ActType { get; set; }
        public string ActorType { get; set; }
        public bool IsAutomation { get; set; }
        public bool? IsLockApprover { get; set; }

        public List<StaffInfo> ActorParms { get; set; }

        public string ActorParmsName { get; set; }
        /// <summary>
        /// 几人通过
        /// </summary>
        public int? HitTimes { get; set; }
        public int? ApproveTimes { get; set; }
        public Guid? EmailTempID { get; set; }
        public int? DueDate { get; set; }


        public List<FlowTempActRouteDto> SwitchPath { get; set; } = new List<FlowTempActRouteDto>();
    }
    public class FlowTempActRouteDto : Entity<Guid>
    {
        public Guid? FlowTempID { get; set; }
        public Guid? ActID { get; set; }
        public string? ActInsName { get; set; }

        public string RouteExecution { get; set; }
        public string Action { get; set; }
        public Guid? NextActID { get; set; }
        public string? NextActInsName { get; set; }

        public string RunLogic { get; set; }
        public int? Sort { get; set; }
    }


    public class FlowInstEntity :Entity<Guid>// AuditEntity<Guid>
    {

        public override Guid Id { get; set; }
        public Guid? FlowTempId { get; set; }

        public string FlowTempTitle { get; set; }

        public string FlowTempName { get; set; }

        public string FlowNo { get; set; }
        public FlowStatus? FlowStatus { get; set; }
        public int? FlowStep { get; set; }
        public string FlowStepName { get; set; }
        public string FlowStepTitle { get; set; }

        public string TaskSubject { get; set; }
        public FlowNoticeType? NoticeType { get; set; }

        public string Applicant { get; set; }

        public string ApplicantName { get; set; }


        public List<StaffInfo> Approver { get; set; }

        public string ApproverName { get; set; }
        public string FormContext { get; set; }
        public string FormData { get; set; }
        public DateTimeOffset? CompleteTime { get; set; }
        public Guid? EmailTempID { get; set; }
        public string CallBackUrl { get; set; }

        public string CreatedByName { get; set; }

        public List<FlowActInstEntity> FlowActs { get; set; } = new List<FlowActInstEntity>();

    }
    public class FlowActInstEntity : Entity<Guid>
    {
        [Column( "ID")]
        public override Guid Id { get; set; }

        [Column( "FlowInstID")]
        public Guid? FlowInstId { get; set; }
        public Guid? FlowTempActID { get; set; }
        public int? ActStep { get; set; }
        public string ActName { get; set; }
        public string ActTitle { get; set; }

        [Column( "Approver")]
        public List<StaffInfo> Approver { get; set; }
        public string ApproverName { get; set; }
        public ActivityStatus? ActStatus { get; set; }
        public string Remark { get; set; }
        public string Transferor { get; set; }
        public string TransferorName { get; set; }
        //[Column(TypeName = "datetimeoffset")]
        public DateTimeOffset? CompleteTime { get; set; }
        public Guid? EmailTempID { get; set; }
        public string FromUrl { get; set; }
        public int? DueDate { get; set; }

    }


    public class FlowBackDataEntity
    {
        public FlowInstEntity FlowInst { get; set; }
        public FlowActInstDto CurrentAct { get; set; }
        public FlowAuditDto FlowAuditRecord { get; set; }
        public FlowAction ActionType { get; set; }

    }
    public partial class FlowActInstDto : Entity<Guid>
    {
        public Guid? FlowInstId { get; set; }
        //public Guid? FlowTempActID { get; set; }
        public int? ActStep { get; set; }
        public string ActName { get; set; }
        public string ActTitle { get; set; }
        public string Approver { get; set; }
        public string ApproverName { get; set; }
        public ActivityStatus? ActStatus { get; set; }
        public string Remark { get; set; }
        //public string Transferor { get; set; }
        //public string TransferorName { get; set; }
        //[Column(TypeName = "datetimeoffset")]
        public DateTimeOffset? CompleteTime { get; set; }
        public Guid? EmailTempID { get; set; }
        //public string FromUrl { get; set; }
        public int? DueDate { get; set; }
    }
}
