using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTempActRoute.Dtos;

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
    public partial class FlowTempActDto : PageEntity<Guid>
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
        public int? ConsolidatedTimes { get; set; } = 1;
        public bool? EmailCcApplicant { get; set; } = false;
        public Guid? EmailTempID { get; set; }
        public int? DueDate { get; set; }

        /// <summary>
        /// 拒绝邮件
        /// </summary>
        public Guid? RejectEmailTempID { get; set; }
        /// <summary>
        /// 抄送通知人
        /// </summary>
        public List<StaffInfo> NoticeUser { get; set; }


        public List<FlowTempActRouteDto> SwitchPath { get; set; }=new List<FlowTempActRouteDto>();
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
}
