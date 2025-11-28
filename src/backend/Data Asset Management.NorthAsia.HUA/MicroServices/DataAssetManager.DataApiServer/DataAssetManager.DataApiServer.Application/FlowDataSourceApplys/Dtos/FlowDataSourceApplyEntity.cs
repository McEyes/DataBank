using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application.FlowDataSourceApplys.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    //[ElasticIndexName("MetaDataUser", "DataAsset")]
    [SugarTable("ITPortal_FlowDataSourceApplys")]
    public class FlowDataSourceApplyEntity : AuditEntity<Guid>, ICreateNameEntity
    {
        [SugarColumn(IsPrimaryKey = true,ColumnName ="Id")]
        public override Guid Id { get; set; }
        /// <summary>
        /// 申请人id
        /// </summary>
        [SugarColumn( ColumnName = "UserId")]
        public string UserId { get; set; }
        /// <summary>
        /// 申请人名称
        /// </summary>
        [SugarColumn(ColumnName = "UserName")]
        public string UserName { get; set; }
        /// <summary>
        /// 申请人邮箱
        /// </summary>
        [SugarColumn(ColumnName = "UserMail")]
        public string UserMail { get; set; }
        //public string MasterName { get; set; }
        //public string MasterName2 { get; set; }
        //public string UserEmail { get; set; }
        //public string DataName { get; set; }
        /// <summary>
        /// 数据来源系统
        /// </summary>
        [SugarColumn(ColumnName = "DataSourse")]
        public string DataSourse { get; set; }
        /// <summary>
        /// 申请目的
        /// </summary>
        [SugarColumn(ColumnName = "ApplyPurpose")]
        public string ApplyPurpose { get; set; }
        //[SugarColumn(ColumnName = "ApplyFlowType")]
        //public int ApplyFlowType { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        [SugarColumn(ColumnName = "FormNo")]
        public string FormNo { get; set; }
        [SugarColumn(IsJson = true,ColumnName = "MasterData")]
        public List<SourseSystemDto> MasterData { get; set; }
        /// <summary>
        /// 同步频率
        /// </summary>
        [SugarColumn(ColumnName = "SyncFrequency")]
        public string SyncFrequency { get; set; }
        /// <summary>
        /// 同步类型：下拉选择
        /// </summary>
        [SugarColumn(ColumnName = "SyncType")]
        public int? SyncType { get; set; }
        /// <summary>
        /// 更新方式：下拉选择
        /// </summary>
        [SugarColumn(ColumnName = "UpdateMethod")]
        public int? UpdateMethod { get; set; }
        [SugarColumn(ColumnName = "SyncFrequencyField")]
        public string SyncFrequencyField { get; set; }
        [SugarColumn(ColumnName = "Comment")]
        public string Comment { get; set; }
        //[SugarColumn(ColumnName = "ExtraProperties")]
        //public string ExtraProperties { get; set; }
        //[SugarColumn(ColumnName = "ConcurrencyStamp")]
        //public string ConcurrencyStamp { get; set; }

        /// <summary>
        /// 主管用户id
        /// </summary>
        //[SugarColumn(ColumnName = "Superior")]
        public string Superior { get; set; }
        /// <summary>
        /// 主管用户id
        /// </summary>
        [SugarColumn(ColumnName = "SuperiorName")]
        public string SuperiorName { get; set; }
        /// <summary>
        /// 数据来源系统SME，单选
        /// </summary>
        [SugarColumn(ColumnName = "SMEUserId")]
        public string SMEUserId { get; set; }
        /// <summary>
        /// 数据来源系统SME名称
        /// </summary>
        [SugarColumn(ColumnName = "SMEUserName")]
        public string SMEUserName { get; set; }

        /// <summary>
        /// 数据来源系统SME，单选
        /// </summary>
        [SugarColumn(ColumnName = "BASUserId")]
        public string BASUserId { get; set; }
        /// <summary>
        /// 数据来源系统SME名称
        /// </summary>
        [SugarColumn(ColumnName = "BASUserName")]
        public string BASUserName { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        [SugarColumn(ColumnName = "Approver")]
        public string Approver { get; set; }
        /// <summary>
        /// 审批人名称
        /// </summary>
        [SugarColumn(ColumnName = "ApproverName")]
        public string ApproverName { get; set; }


        [SugarColumn(ColumnName = "CreatedByName")]
        public string CreatedByName { get; set; }
        /// <summary>
        /// 流程发起状态
        /// </summary>
        [SugarColumn(ColumnName = "Status")]
        public int Status { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<FlowAttachmentInfo> FileList { get; set; }
    }
}
