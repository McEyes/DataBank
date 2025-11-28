using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowInstActRoute.Dtos;
using ITPortal.Flow.Application.FlowTempAct.Dtos;
using ITPortal.Flow.Application.FlowTempActRoute.Dtos;
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
    public partial class FlowActInstDto:PageEntity<Guid>
    {

        public Guid? FlowInstId { get; set; }
        public Guid? FlowTempActID { get; set; }
        public int? ActStep { get; set; }
        public string ActName { get; set; }
        public string ActTitle { get; set; }
        /// <summary>
        /// 节点类型：Start，End, Normal, Notify,EmailNotify,SMSNotify,Http,
        /// </summary>
        public string ActType { get; set; }
        public string Approver { get; set; }
        public string ApproverName { get; set; }
        public ActivityStatus? ActStatus { get; set; }
        public string Remark { get; set; }
        public string Transferor { get; set; }
        public string TransferorName { get; set; }
        //[Column(TypeName = "datetimeoffset")]
        public DateTimeOffset? CompleteTime { get; set; }

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
        public string FromUrl { get; set; }
        public int? DueDate { get; set; }
        public int? HitTimes { get; set; }
        public int? ApprovedTimes { get; set; }
        public int? ConsolidatedTimes { get; set; } = 1;
        public bool? EmailCcApplicant { get; set; } = false;
        public List<FlowInstActRouteDto> SwitchPath { get; set; } = new List<FlowInstActRouteDto>();
    }
}
