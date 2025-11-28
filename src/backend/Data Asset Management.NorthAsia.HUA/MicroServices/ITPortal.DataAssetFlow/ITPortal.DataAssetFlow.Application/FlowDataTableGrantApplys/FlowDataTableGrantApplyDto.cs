using ElsaDemo.Shared.ApplyFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jb.home.Service.Application.Contracts.FlowDataTableGrantApplys
{
    public class FlowDataTableGrantApplyDto
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserNtid { get; set; }
        public string SmeId { get; set; }
        public string SmeEmail { get; set; }
        public string SmeName { get; set; }
        public DateTime? RequireDate { get; set; }
        public string ApplyFormNo { get; set; }
        public string ApplyFormId { get; set; }
        public string Tables { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string MainDomain { get; set; }
        public bool IsSmeToVote { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        public string Reason { get; set; }
        //public ApplyFlowApproveRecordDto ApplyFlowApproveRecord { get; set; }
        public ApplyFlowBasicInfoDto ApplyFlowBasicInfo { get; set; }

        /// <summary>
        /// 节点信息
        /// </summary>
        public List<ApplyFlowCommonNodeDto> NodeList { get; set; }
    }
}
