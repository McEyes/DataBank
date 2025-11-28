using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.ApproveFlow.Dtos
{
    public class ApprovalNodeDto
    {
        public Guid id { get; set; }
        public int flowStep { get; set; }
        public string flowStepName { get; set; }
        public string flowStepTitle { get; set; }
        public string applicant { get; set; }
        public string applicantName { get; set; }
        public List<ApprovalNodeApproverDto> approver { get; set; }
    }

    public class ApprovalNodeApproverDto
    {
        public string ntid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string department { get; set; }
    }
}
