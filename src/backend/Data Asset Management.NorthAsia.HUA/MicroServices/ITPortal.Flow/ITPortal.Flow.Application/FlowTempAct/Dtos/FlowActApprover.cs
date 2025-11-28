using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTempActRoute.Dtos;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowTempAct.Dtos
{
    public partial class FlowActApprover : Entity<Guid>
    {

        public int? ActStep { get; set; }
        public string ActName { get; set; }
        public string ActTitle { get; set; }

        public List<StaffInfo> ActorParms { get; set; }= new List<StaffInfo>();

        public string ActorParmsName { get; set; }

    }

}
