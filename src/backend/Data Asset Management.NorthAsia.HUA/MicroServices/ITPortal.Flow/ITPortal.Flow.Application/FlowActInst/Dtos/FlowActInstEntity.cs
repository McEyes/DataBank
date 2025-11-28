using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowInstActRoute.Dtos;
using ITPortal.Flow.Application.FlowTempAct.Dtos;
using ITPortal.Flow.Core;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowActInst.Dtos
{
    [SugarTable("FlowActInst")]
    public partial class FlowActInstEntity : AuditEntity<Guid>
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }

        [SugarColumn(ColumnName = "FlowInstID")]
        public Guid? FlowInstId { get; set; }
        public Guid? FlowTempActID { get; set; }
        public int? ActStep { get; set; }
        public string ActName { get; set; }
        public string ActTitle { get; set; }
        /// <summary>
        /// 节点类型：Start，End, Normal, Notify,EmailNotify,SMSNotify,Http,
        /// </summary>
        public string ActType { get; set; }

        [SugarColumn(ColumnName = "Approver", IsJson = true)]
        public List<StaffInfo> Approver { get; set; }
        public string ApproverName { get; set; }
        public ActivityStatus? ActStatus { get; set; }
        public string Remark { get; set; }
        public string Transferor { get; set; }
        public string TransferorName { get; set; }
        //[Column(TypeName = "datetimeoffset")]
        public DateTimeOffset? CompleteTime { get; set; }
        public string FromUrl { get; set; }
        public int? DueDate{ get; set; }
        public int? HitTimes { get; set; }
        public int? ApprovedTimes { get; set; } = 0;
        public int? ConsolidatedTimes { get; set; } = 1;
        public bool? EmailCcApplicant { get; set; } = false;

        public Guid? EmailTempID { get; set; }
        /// <summary>
        /// 拒绝邮件
        /// </summary>
        public Guid? RejectEmailTempID { get; set; }
        /// <summary>
        /// 抄送通知人
        /// </summary>
        [SugarColumn(ColumnName = "NoticeUser", IsJson = true)]
        public List<StaffInfo> NoticeUser { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<FlowInstActRouteEntity> SwitchPath { get; set; } = new List<FlowInstActRouteEntity>();
    }
}
