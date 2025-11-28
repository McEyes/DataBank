using jb.home.Application.Contracts.FlowDataSourceApplys;
using jb.home.ApplyFlowLogs;
using jb.home.FlowDataSourceSMEs;
using jb.home.Service.Application.Contracts.ApplyFlowAttacchments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jb.home.FlowDataSourceApplys
{
    public class FlowDataSourceApplyDto
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserMail { get; set; }
        public string? MasterName { get; set; }
        public string? MasterName2 { get; set; }
        //public string UserEmail { get; set; }
        //public string DataName { get; set; }
        public string DataSourse { get; set; }
        public string MasterData { get; set; }
        public string SyncFrequency { get; set; }
        public int? SyncType { get; set; }
        public int? UpdateMethod { get; set; }
        public string ApplyPurpose { get; set; }
        public int ApplyFlowType { get; set; }
        /// <summary>
        /// 是否可以审核
        /// </summary>
        public bool IsToVote { get; set; }
        /// <summary>
        /// 是否可以上传附件
        /// </summary>
        public bool IsUploadFile { get; set; }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsToEdit { get; set; }
        /// <summary>
        /// 是否可取消
        /// </summary>
        public bool IsToCancel { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 当前节点
        /// </summary>
        public int CurrNodeIndex { get; set; }
        /// <summary>
        /// 上一个节点通过状态
        /// </summary>
        public string PrevNodeName { get; set; }


        /// <summary>
        /// 单号
        /// </summary>
        [MaxLength(64)]
        public string FormNo { get; set; }
        public List<FlowDataSourceSMEDto> SmeList { get; set; }
        public List<ApplyFlowLogDto> ApplyFlowLogs { get; set; }
        public List<ApplyFlowAttacchmentDto> FileList { get; set; }
        public List<SourseSystemDto> MasterDataList { get; set; }
        public List<ApplyOtherFlowAttacchmentDto> OtherFileList { get; set; }
    }
}
