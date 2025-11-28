using System;
namespace jb.home.Service.Application.Contracts.ApplyFlowAttacchments
{
    public class CreateApplyFlowAttacchmentInputDto
    {
        public Guid? Id { get; set; }

            public Guid ApplyFlowInfoId { get; set; }
            public string FileName { get; set; }
            public string FileUrl { get; set; }
            public string FileType { get; set; }


        //public List<ApplyFlowAttacchmentItemDto> ChildList { get; set; }
    }
}