using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowActInst.Dtos;
using ITPortal.Flow.Application.FlowTempAct.Dtos;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowInst.Dtos
{
    public partial class FlowInstDto :PageEntity<Guid>
    {
        public Guid? FlowTempId { get; set; }

        public string FlowTempTitle { get; set; }

        public string FlowTempName { get; set; }

        public string FlowNo { get; set; }
        public int? FlowStatus { get; set; }
        public int? FlowStep { get; set; }
        public string FlowStepName { get; set; }
        public string FlowStepTitle { get; set; }
        public string TaskSubject { get; set; }
        public int? NoticeType { get; set; }

        public string Applicant { get; set; }

        public string ApplicantName { get; set; }

        public string ApplicantEmail { get; set; }

        public string Approver { get; set; }

        public string ApproverName { get; set; }
        public string FormContext { get; set; }
        public string FormData { get; set; }
        public DateTimeOffset? CompleteTime { get; set; }
        public Guid? EmailTempID { get; set; }

        public string CreatedByName { get; set; }
        public string CallBackUrl { get; set; }

        public Guid? FinishEmailTempID { get; set; }
        /// <summary>
        /// 拒绝邮件
        /// </summary>
        public Guid? RejectEmailTempID { get; set; }
        /// <summary>
        /// 抄送通知人
        /// </summary>
        public List<StaffInfo> EndNoticeUser { get; set; }
        public List<FlowActInstDto> FlowActs { get; set; } = new List<FlowActInstDto>();
    }
}
