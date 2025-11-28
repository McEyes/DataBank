using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.FlowAuditRecord.Dtos
{
    [SugarTable("FlowAuditRecord")]
    public partial class FlowAuditRecordEntity : AuditEntity<Guid>
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }
       [SugarColumn(ColumnName = "FlowInstID")]
        public Guid? FlowInstId { get; set; }
        public Guid? ActId { get; set; }
        public int? ActStep { get; set; }
        public string ActTitle { get; set; }
        public string ActName { get; set; }
        public string Approver { get; set; }
        public string ApproverName { get; set; }
        public string ActOperate { get; set; }
        public string AuditContent { get; set; }
        public string Remark { get; set; }
        public string Transferor { get; set; }
        public string TransferorName { get; set; }
        //[SugarColumn(IsIgnore =true)]
        //public DateTimeOffset? CompleteTime { get; set; }

    }
}
