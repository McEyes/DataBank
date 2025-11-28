using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowInstActRoute.Dtos
{
    [SugarTable("FlowInstActRoute")]
    public partial class FlowInstActRouteEntity : AuditEntity<Guid>
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }
        [SugarColumn(ColumnName = "FlowInsID")]
        public Guid FlowInsID { get; set; }
        public Guid? ActInsID { get; set; }
        public string? ActInsName { get; set; }

        public string RouteExecution { get; set; }
        public string Action { get; set; }
        public Guid? NextActInsID { get; set; }
        public string? NextActInsName { get; set; }

        public string RunLogic { get; set; }
        public int? Sort { get; set; }
        //public DateTimeOffset? CompleteTime { get; set; }
    }
}
