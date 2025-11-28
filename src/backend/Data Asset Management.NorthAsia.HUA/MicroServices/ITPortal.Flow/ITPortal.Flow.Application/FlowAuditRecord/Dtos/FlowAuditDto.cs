using ITPortal.Flow.Application.FlowAttachments.Dtos;
using ITPortal.Flow.Application.FlowTempAct.Dtos;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowAuditRecord.Dtos
{
    public partial class FlowAuditDto
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
        /// 审批动作: Submit 提交，Approval 审批(同意，通过等)，Reject 驳回，RejectStart 驳回到开始，RejectEnd 拒绝（流程直接结束）
        /// </summary>
        public string ActOperate { get; set; }
        /// <summary>
        /// 审批内容
        /// </summary>
        public string AuditContent { get; set; }

        /// <summary>
        /// 审批人配置，只能配置之后的节点，非必填
        /// </summary>
        public List<FlowActApprover> FlowActs { get; set; } = new List<FlowActApprover>();
        ///// <summary>
        ///// 审批备注
        ///// </summary>
        //public string Remark { get; set; }
        //public string Transferor { get; set; }
        //public string TransferorName { get; set; }
        //附件
        public List<FlowAttachmentInfo> Attacchments { get; set; } = new List<FlowAttachmentInfo>();
    }
    public partial class FlowTransferAuditDto: FlowAuditDto
    {
        /// <summary>
        /// 转办人
        /// </summary>
        public string Transferor { get; set; }
        public string TransferorName { get; set; }
        //附件
    }
    /// <summary>
    /// 重启流程跳转到指定节点
    /// </summary>
    public partial class FlowGotToActAuditDto : FlowAuditDto
    {
        /// <summary>
        /// 启动到指定节点
        /// </summary>
        public string ActName { get; set; }
    }
}
