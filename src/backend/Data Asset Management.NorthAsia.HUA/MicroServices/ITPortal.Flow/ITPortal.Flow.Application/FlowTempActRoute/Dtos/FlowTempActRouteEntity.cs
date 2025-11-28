using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowTempActRoute.Dtos
{
    [SugarTable("FlowTempActRoute")]
    public partial class FlowTempActRouteEntity : AuditEntity<Guid>
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }
        [SugarColumn(ColumnName = "FlowTempID")]
        public Guid? FlowTempID { get; set; }
        public Guid? ActID { get; set; }
        public string? ActInsName { get; set; }

        public string RouteExecution { get; set; }
        public string Action { get; set; }
        public Guid? NextActID { get; set; }
        public string? NextActInsName { get; set; }

        public string RunLogic { get; set; }
        public int? Sort { get; set; }
    }
}
