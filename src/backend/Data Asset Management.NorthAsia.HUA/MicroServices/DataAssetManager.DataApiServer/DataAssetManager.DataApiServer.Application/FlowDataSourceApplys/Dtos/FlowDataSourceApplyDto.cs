using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application.FlowDataSourceApplys.Dtos
{
    public class FlowDataSourceApplyDto:PageEntity<Guid>
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        //public string UserMail { get; set; }
        //public string MasterName { get; set; }
        //public string MasterName2 { get; set; }
        //public string UserEmail { get; set; }
        //public string DataName { get; set; }
        public string DataSourse { get; set; }
        public string ApplyPurpose { get; set; }
        //public int ApplyFlowType { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string FormNo { get; set; }
        [SugarColumn(IsJson = true)]
        public List<SourseSystemDto> MasterData { get; set; }
        public string SyncFrequency { get; set; }
        public int? SyncType { get; set; }
        public int? UpdateMethod { get; set; }
        public string SyncFrequencyField { get; set; }
        public string Comment { get; set; }
        //public string ExtraProperties { get; set; }
        //public string ConcurrencyStamp { get; set; }


        public string SMEUserId { get; set; }
        public string SMEUserName { get; set; }

        /// <summary>
        /// 主管用户id
        /// </summary>
        public string Superior { get; set; }
        /// <summary>
        /// 主管用户id
        /// </summary>
        public string SuperiorName { get; set; }

        /// <summary>
        /// 数据来源系统SME，单选
        /// </summary>
        public string BASUserId { get; set; }
        /// <summary>
        /// 数据来源系统SME名称
        /// </summary>
        public string BASUserName { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string Approver { get; set; }
        /// <summary>
        /// 审批人名称
        /// </summary>
        public string ApproverName { get; set; }

        public string CreatedByName { get; set; }
        public int Status { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<FlowAttachmentInfo> FileList { get; set; }
    }
}
