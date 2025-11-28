using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowActInst.Dtos;
using ITPortal.Flow.Application.FlowTempAct.Dtos;
using ITPortal.Flow.Core;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowInst.Dtos
{
    [SugarTable("FlowInst")]
    public partial class FlowInstEntity : AuditEntity<Guid>, ICreateNameEntity
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }
        [SugarColumn(ColumnName = "FlowTempID")]
        public Guid? FlowTempId { get; set; }

        public string FlowTempTitle { get; set; }

        public string FlowTempName { get; set; }

        public string FlowNo { get; set; }
        public FlowStatus? FlowStatus { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string FlowStatusName { get { return FlowStatus.ToString(); } }
        public int? FlowStep { get; set; }
        public string FlowStepName { get; set; }
        public string FlowStepTitle { get; set; }

        public string TaskSubject { get; set; }
        public FlowNoticeType? NoticeType { get; set; }

        public string Applicant { get; set; }

        public string ApplicantName { get; set; }

        public string ApplicantEmail { get; set; }


        [SugarColumn(ColumnName = "Approver", IsJson = true)]
        public List<StaffInfo> Approver { get; set; }

        public string ApproverName { get; set; }
        public string FormContext { get; set; }
        public string FormData { get; set; }
        public DateTimeOffset? CompleteTime { get; set; }
        /// <summary>
        /// 代办邮件
        /// </summary>
        public Guid? EmailTempID { get; set; }
        /// <summary>
        /// 拒绝邮件
        /// </summary>
        public Guid? RejectEmailTempID { get; set; }
        /// <summary>
        /// 流程结束完成邮件
        /// </summary>
        public Guid? FinishEmailTempID { get; set; }
        /// <summary>
        /// 流程结束通知人
        /// </summary>
        [SugarColumn(ColumnName = "EndNoticeUser", IsJson = true)]
        public List<StaffInfo> EndNoticeUser{ get; set; }
        public string CallBackUrl { get; set; }

        /// <summary>
        /// 流程表单url
        /// </summary>
        public string FormUrl { get; set; }

        public string CreatedByName { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<FlowActInstEntity> FlowActs { get; set; } = new List<FlowActInstEntity>();
    }
}
