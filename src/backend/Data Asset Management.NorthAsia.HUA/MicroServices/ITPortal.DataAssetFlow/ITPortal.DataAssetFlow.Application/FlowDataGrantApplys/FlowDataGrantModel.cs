namespace jb.home.Service.Application.Contracts.FlowDataGrantApplys
{
    public class FlowDataGrantModel
    {
        public Guid UserId { get; set; }
        public string SMEId { get; set; }
        public string SMEEmail { get; set; }
        public string? SMEName { get; set; }
        /// <summary>
        /// 暂存人员名称,后端获取
        /// </summary>
        public string? UserName { get; set; }    
        /// <summary>
        /// 申请表名
        /// </summary>
        public List<string> Tables { get; set; }
        /// <summary>
        /// 需要审批完成时间
        /// </summary>
        public DateTime? RequireDate { get; set; }
        public string? ApplyFormNo { get; set; }
        public string ApplyFormId { get; set; }
        public string Remark { get; set; }

        

        
        /// <summary>
        /// 目的
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 节点
        /// </summary>
        public List<ApplyFlowCommonNodeInput> NodeList { get; set; }
    }
}
