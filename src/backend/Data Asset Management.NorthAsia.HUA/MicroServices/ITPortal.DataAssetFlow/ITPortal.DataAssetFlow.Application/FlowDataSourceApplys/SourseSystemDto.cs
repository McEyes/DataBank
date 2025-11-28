using jb.home.FlowDataSourceSMEs;
using jb.home.Service.Application.Contracts.ApplyFlowAttacchments;
using System;
using System.Numerics;
namespace jb.home.Application.Contracts.FlowDataSourceApplys
{
    public class SourseSystemDto
    {
        public string MasterData { get; set; }
        public string SourceSystem { get; set; }
        public string DataSource { get; set; }
        public bool? NeedStandard { get; set; }
        public bool? IsMapping { get; set; }
    }
}