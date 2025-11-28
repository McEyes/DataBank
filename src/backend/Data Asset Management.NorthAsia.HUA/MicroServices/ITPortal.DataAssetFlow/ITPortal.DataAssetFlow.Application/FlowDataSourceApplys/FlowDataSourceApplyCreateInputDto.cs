using jb.home.FlowDataSourceSMEs;
using jb.home.Service.Application.Contracts.ApplyFlowAttacchments;
using System;
namespace jb.home.Application.Contracts.FlowDataSourceApplys
{
    public class CreateFlowDataSourceApplyInputDto
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string DataName { get; set; }
        public string DataSourse { get; set; }
        public string MasterData { get; set; }
        public string ApplyPurpose { get; set; }
        public string SyncFrequency { get; set; }
        public int? SyncType { get; set; }
        public int? UpdateMethod { get; set; }
        public int ApplyFlowType { get; set; }
        public string? Comment { get; set; }
        public List<FlowDataSourceSMEDto> SmeList { get; set; }
        public List<ApplyFlowAttacchmentDto> FileList { get; set; }
        public List<SourseSystemDto> MasterDataList { get; set; }
        public List<ApplyOtherFlowAttacchmentDto> OtherFileList { get; set; }
    }
}