using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowAttachments.Dtos;
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
    public partial class StartFlowDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 流程模板id，指定具体模板时，即使该模板非有效状态也可以创建
        /// </summary>
        public string FlowTempId { get; set; }
        /// <summary>
        /// 模板名称，自动获取最新的有效模板创建流程实例
        /// </summary>
        public string FlowTempName { get; set; }

        /// <summary>
        /// 表单数据id，也会作为flowid
        /// </summary>
        public Guid FormId { get; set; }
        /// <summary>
        /// 为空时，根据template规则自动生成
        /// </summary>
        public string FormNo { get; set; }
        /// <summary>
        /// 表单数据，用来构建邮件模板和标题模板
        /// </summary>
        public dynamic FormData { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string Applicant { get; set; }

        /// <summary>
        /// 申请人名称
        /// </summary>
        public string ApplicantName { get; set; }

        public string ApplicantEmail { get; set; }


        /// <summary>
        /// 指定节点审批人
        /// 只需要指定具体节点名称和审批人即可
        /// </summary> 
        public List<FlowActApprover> ActApprovers { get; set; } = new List<FlowActApprover>();
        /// <summary>
        /// 流程附件，统一管理
        /// </summary>
        public List<FlowAttachmentInfo> Attacchments { get; set; } = new List<FlowAttachmentInfo>();
    }
}
