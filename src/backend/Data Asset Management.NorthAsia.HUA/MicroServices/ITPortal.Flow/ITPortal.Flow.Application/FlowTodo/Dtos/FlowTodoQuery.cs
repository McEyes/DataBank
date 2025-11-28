using ITPortal.Core.Services;
using ITPortal.Flow.Core;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowTodo.Dtos
{
    public partial class FlowTodoQuery : PageEntity<Guid>
    {

        public string FlowNo { get; set; }
        public Guid? FlowTempID { get; set; }


        public string OwnerID { get; set; }
    
        public string Title { get; set; }

        public string Applicant { get; set; }

        public string ApplicantName { get; set; }

        public string Approver { get; set; }

        public string ApproverName { get; set; }

        public string Remark { get; set; }
        public FlowStatus? FlowStatus { get; set; }
        public TodoStatus? Status { get; set; }
        public DateTimeOffset? CompleteTime { get; set; }
    }
}
