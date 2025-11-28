using Furion;
using Furion.DependencyInjection;
using Furion.HttpRemote;

using DataTopicStore.Application.ApproveFlow.Dto;


using SqlSugar;
using DataTopicStore.Application.ApproveFlow.Entities;
using DataTopicStore.Application.ApproveFlow.Enums;
using DataTopicStore.Application.ApproveFlow.Dtos;
using DataTopicStore.Application.Common.Dtos;


namespace DataTopicStore.Application.ApproveFlow
{

    /// <summary>
    /// data auth apply 流程接口代理
    /// </summary>
    public class FlowApplyProxyService : ITransient
    {
        private readonly IHttpRemoteService httpRemoteService;
        private readonly string BaseHostUrl;
        private string Authorization = "";
        public FlowApplyProxyService(IHttpRemoteService httpRemoteService)
        {
            this.httpRemoteService = httpRemoteService;
            BaseHostUrl = App.GetConfig<string>("RemoteApi:ITPortalFlowUrl");
        }

        private string GetToken()
        {
            var token = string.Empty;
            if (App.HttpContext != null && App.HttpContext.Request.Headers != null)
                token = App.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrWhiteSpace(token)) token = token.Replace("Bearer ", "");
            return token;
        }

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<HttpResult<FlowTemplateDto>?> GetFlowTempInfo(string tempName) => await httpRemoteService.GetAsAsync<HttpResult<FlowTemplateDto>>($"{BaseHostUrl}/api/FlowTemplate/Info/name/{tempName}",
            builder => builder.AddAuthentication("Bearer", GetToken()));


        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<HttpResult<FlowInstEntity>?> InitFlowAsync(StartFlowDto input) => await httpRemoteService.PostAsAsync<HttpResult<FlowInstEntity>>($"{BaseHostUrl}/api/Flow/start",
            builder => builder.SetContent(input, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

        /// <summary>
        /// 发起失败时删除流程
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<HttpResult?> DeleteAsync(string applyFormNo) => await httpRemoteService.PostAsAsync<HttpResult>($"{BaseHostUrl}/api/Flow/DeleteByNo?flowNo={applyFormNo}",
            builder => builder.SetContent(new object(), "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

        /// <summary>
        /// 发起失败时删除流程
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<HttpResult?> DeleteAsync(Guid flowId) => await httpRemoteService.PostAsAsync<HttpResult>($"{BaseHostUrl}/api/Flow/Delete/{flowId}",
            builder => builder.SetContent(new object(), "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

        ///// <summary>
        ///// 流程审批，通用审批
        ///// </summary>
        ///// <param name="applyFormNo"></param>
        ///// <returns></returns>
        //public async Task<HttpResult?> ApproveAsync(ApplyFlowApproveRecordInput approveRecord) => await httpRemoteService.PostAsAsync<HttpResult>($"{BaseHostUrl}/homeapi/api/home/FlowDataAuthApply/SendApprove",
        //    builder => builder.SetContent(new object(), "application/json;charset=utf-8"));

        /// <summary>
        /// 流程审批，通用审批
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<HttpResult?> SendApproveAsync(FlowAuditDto approveRecord) => await httpRemoteService.PostAsAsync<HttpResult<string>>($"{BaseHostUrl}/api/Flow/approval",
            builder => builder.SetContent(approveRecord, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));
        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<HttpResult?> SendRejectAsync(FlowAuditDto approveRecord) => await httpRemoteService.PostAsAsync<HttpResult<string>>($"{BaseHostUrl}/api/Flow/reject",
            builder => builder.SetContent(approveRecord, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));
        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<HttpResult?> SendRejectEndAsync(FlowAuditDto approveRecord) => await httpRemoteService.PostAsAsync<HttpResult<string>>($"{BaseHostUrl}/api/Flow/rejectend",
            builder => builder.SetContent(approveRecord, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));
        /// <summary>
        /// 驳回到开始
        /// </summary>
        /// <param name="applyFormNo"></param>
        /// <returns></returns>
        public async Task<HttpResult?> SendRejectStartAsync(FlowAuditDto approveRecord) => await httpRemoteService.PostAsAsync<HttpResult<string>>($"{BaseHostUrl}/api/Flow/rejectstart",
            builder => builder.SetContent(approveRecord, "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

        /// <summary>
        /// 获取审批流信息
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public async Task<HttpResult<ApprovalNodeDto>> GetFlowInstAsync(Guid flowId) => await httpRemoteService.PostAsAsync<HttpResult<ApprovalNodeDto>>($"{BaseHostUrl}/api/Flow/flowinst/{flowId}",
            builder => builder.SetContent("", "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

    }
}
namespace DataTopicStore.Application.ApproveFlow.Dto
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
    }
    public class FlowActApprover// : Entity<Guid>
    {

        //public int? ActStep { get; set; }
        public string ActName { get; set; }
        //public string ActTitle { get; set; }

        public List<StaffInfo> ActorParms { get; set; } = new List<StaffInfo>();

        public string ActorParmsName { get; set; }

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
        //public Guid Id { get; set; }
        public Guid FlowInstId { get; set; }
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


    public class FlowInstEntity : AuditEntity<Guid>
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
        public DateTime? CompleteTime { get; set; }
        public Guid? EmailTempID { get; set; }
        public string CallBackUrl { get; set; }

        public string CreatedByName { get; set; }

        public List<FlowActInstEntity> FlowActs { get; set; } = new List<FlowActInstEntity>();

    }
    public class FlowActInstEntity : Entity<Guid>
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }

        [SugarColumn(ColumnName = "FlowInstID")]
        public Guid? FlowInstId { get; set; }
        public Guid? FlowTempActID { get; set; }
        public int? ActStep { get; set; }
        public string ActName { get; set; }
        public string ActTitle { get; set; }

        [SugarColumn(ColumnName = "Approver", IsJson = true)]
        public List<StaffInfo> Approver { get; set; }
        public string ApproverName { get; set; }
        public ActivityStatus? ActStatus { get; set; }
        public string Remark { get; set; }
        public string Transferor { get; set; }
        public string TransferorName { get; set; }
        //[Column(TypeName = "datetime")]
        public DateTime? CompleteTime { get; set; }
        public Guid? EmailTempID { get; set; }
        public string FromUrl { get; set; }
        public int? DueDate { get; set; }

    }


    public class FlowBackDataEntity
    {
      public  FlowInstEntity FlowInst {  get; set; }
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
        //[Column(TypeName = "datetime")]
        public DateTime? CompleteTime { get; set; }
        public Guid? EmailTempID { get; set; }
        //public string FromUrl { get; set; }
        public int? DueDate { get; set; }
    }
}
