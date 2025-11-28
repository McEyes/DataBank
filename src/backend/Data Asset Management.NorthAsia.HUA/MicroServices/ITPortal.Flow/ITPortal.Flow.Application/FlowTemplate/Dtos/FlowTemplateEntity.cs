using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTempAct.Dtos;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowTemplate.Dtos
{
    [SugarTable("FlowTemplate")]
    public partial class FlowTemplateEntity : AuditEntity<Guid>
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }
        /// <summary>
        /// 编码规则，前缀
        /// </summary>
        public string FlowNo { get; set; }
        public string FlowName { get; set; }

        public string FlowTitle { get; set; }

        public string TaskTitle { get; set; }
        public string FormTemplate { get; set; }
        //public string FromData { get; set; }
        public string FormUrl { get; set; }
        public string CallBackUrl { get; set; }
        public int? Sort { get; set; }
        public int Status { get; set; }
        
        public string Description { get; set; }
        public int? NoticeType { get; set; }
        
        public string Ver { get; set; }
        public Guid? EmailTempID { get; set; }

        public Guid? FinishEmailTempID { get; set; }
        /// <summary>
        /// 拒绝邮件
        /// </summary>
        public Guid? RejectEmailTempID { get; set; }
        /// <summary>
        /// 抄送通知人
        /// </summary>
        [SugarColumn(ColumnName = "EndNoticeUser", IsJson = true)]
        public List<StaffInfo> EndNoticeUser { get; set; }
    }
}
