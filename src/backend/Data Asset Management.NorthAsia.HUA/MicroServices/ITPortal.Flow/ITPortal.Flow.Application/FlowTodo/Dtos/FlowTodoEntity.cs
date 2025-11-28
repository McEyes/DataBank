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
    [SugarTable("FlowTodo")]
    public partial class FlowTodoEntity : AuditEntity<Guid>
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }
       [SugarColumn(ColumnName = "FlowInstID")]
        public Guid? FlowInstID { get; set; }
        
        public string FlowNo { get; set; }
       [SugarColumn(ColumnName = "FlowTempID")]
        public Guid? FlowTempID { get; set; }
        
        public string FlowTempName { get; set; }
        
        public string OwnerID { get; set; }
       [SugarColumn(ColumnName = "ActID")]
        public Guid? ActID { get; set; }
        public int? ActStep { get; set; }
        
        public string ActTitle { get; set; }
        
        public string ActName { get; set; }
        
        public string Title { get; set; }
        
        public string Context { get; set; }
        
        public string Applicant { get; set; }
        
        public string ApplicantName { get; set; }
        
        public string Approver { get; set; }
        
        public string ApproverName { get; set; }
        
        public string Remark { get; set; }
        public FlowStatus FlowStatus { get; set; }
        public TodoStatus Status { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string StatusName { get { return Status.ToString(); } }
        public FlowNoticeType? NoticeType { get; set; }

       //[SugarColumn(ColumnName =)]
        public DateTimeOffset? CompleteTime { get; set; }
        
        public string Category { get; set; }
        
        public string SubCategory { get; set; }
    }
}
