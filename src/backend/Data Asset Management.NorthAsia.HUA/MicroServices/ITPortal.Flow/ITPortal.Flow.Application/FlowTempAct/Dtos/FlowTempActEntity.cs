using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowTempAct.Dtos
{
    [SugarTable("FlowTempAct")]
    public partial class FlowTempActEntity : AuditEntity<Guid>
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }
       [SugarColumn(ColumnName = "FlowTempID")]
        public Guid? FlowTempID { get; set; }
        public int? ActStep { get; set; }
        public string ActName { get; set; }
        public string ActTitle { get; set; }
        public string FormUrl { get; set; }

        /// <summary>
        /// 节点类型：Start，End, Normal, Notify,EmailNotify,SMSNotify,Http,
        /// </summary>
        public string ActType { get; set; }
        public string ActorType { get; set; }
        public bool IsAutomation { get; set; }

        [SugarColumn(ColumnName = "ActorParms", IsJson = true)]
        public List<StaffInfo> ActorParms { get; set; }
        
        public string ActorParmsName { get; set; }
        public int? HitTimes { get; set; }
        public int? ApproveTimes { get; set; }
        public int? ConsolidatedTimes { get; set; } = 1;
        public bool? EmailCcApplicant { get; set; } = false;
        public Guid? EmailTempID { get; set; }
        public int? DueDate { get; set; }
        public bool? IsLockApprover { get; set; }

        /// <summary>
        /// 拒绝邮件
        /// </summary>
        public Guid? RejectEmailTempID { get; set; }
        /// <summary>
        /// 抄送通知人
        /// </summary>
        [SugarColumn(ColumnName = "NoticeUser", IsJson = true)]
        public List<StaffInfo> NoticeUser { get; set; }

    }
}
