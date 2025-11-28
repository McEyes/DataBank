using System;
namespace jb.home.Service.Application.Contracts.FlowDataTableGrantApplys
{
    public class CreateFlowDataTableGrantApplyInputDto
    {
        public Guid? Id { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserNtid { get; set; }
        public string SmeId { get; set; }
        public string SmeEmail { get; set; }
        public DateTime? RequireDate { get; set; }
        public string ApplyFormNo { get; set; }
        public string ApplyFormId { get; set; }
        public string Tables { get; set; }
        public string Remark { get; set; }


    }
}