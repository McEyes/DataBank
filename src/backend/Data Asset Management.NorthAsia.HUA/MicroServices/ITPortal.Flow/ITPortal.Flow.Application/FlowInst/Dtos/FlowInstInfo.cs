using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowActInst.Dtos;
using ITPortal.Flow.Application.FlowAttachments.Dtos;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;
using ITPortal.Flow.Application.FlowTempAct.Dtos;
using ITPortal.Flow.Core;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowInst.Dtos
{
    public partial class FlowInstInfo : Entity<Guid>
    {
        public Guid? FlowTempId { get; set; }

        public string FlowTempTitle { get; set; }

        public string FlowTempName { get; set; }

        public string FlowNo { get; set; }
        public int? FlowStatus { get; set; }
        public string FlowStatusName { get { return ((FlowStatus)FlowStatus).ToString(); } }
        //public int? FlowStep { get; set; }
        public string FlowStepName { get; set; }
        public string FlowStepTitle { get; set; }
        public string TaskSubject { get; set; }
        public int? NoticeType { get; set; }
        public string FormData { get; set; }

        public string Applicant { get; set; }

        public string ApplicantName { get; set; }

        public string ApplicantEmail { get; set; }

        public List<StaffInfo> Approver { get; set; }

        public string ApproverId
        {
            get
            {
                if (Approver == null) return string.Empty;
                else return string.Join(";", Approver.Select(f => f.Ntid).ToList());
            }
        }

        public string ApproverName { get; set; }
        public DateTimeOffset? CompleteTime { get; set; }

        /// <summary>
        /// createTime
        /// </summary>
        public  DateTimeOffset CreateTime { get; set; }
        /// <summary>
        /// createBy
        /// </summary>
        public  string CreateBy { get; set; }
        public string CreatedByName { get; set; }
        public string CallBackUrl { get; set; }

        /// <summary>
        /// 流程表单url
        /// </summary>
        public string FormUrl { get; set; }

        public Guid? FinishEmailTempID { get; set; }
        /// <summary>
        /// 拒绝邮件
        /// </summary>
        public Guid? RejectEmailTempID { get; set; }
        /// <summary>
        /// 抄送通知人
        /// </summary>
        public List<StaffInfo> EndNoticeUser { get; set; }

        /// <summary>
        /// 审批节点信息
        /// </summary>
        public List<FlowActInstEntity> FlowActs { get; set; } = new List<FlowActInstEntity>();

        /// <summary>
        /// 审批记录
        /// </summary>
        public List<FlowAuditRecordEntity> AuditRecords { get; set; } = new List<FlowAuditRecordEntity>();

        /// <summary>
        /// 流程附件
        /// </summary>
        public List<FlowAttachmentInfo> Attacchments { get; set; } = new List<FlowAttachmentInfo>();
    }
}
