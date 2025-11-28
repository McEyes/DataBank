using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.EmailTemplate.Dtos
{
    [SugarTable("EmailTemplate")]
    public partial class EmailTemplateEntity : AuditEntity<Guid>
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }

        [SugarColumn(ColumnName = "EmailName")]
        public string EmailName { get; set; }
        [SugarColumn(ColumnName = "EmailTitle")]
        public string EmailTitle { get; set; }

        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        [SugarColumn(ColumnName = "Bcc")]
        public string EmailBcc { get; set; }
        public string EmailSubject { get; set; }
        public string EmailTemplate { get; set; }


        public int? Sort { get; set; }
        public bool? IsEnabled { get; set; }
        public string Remark { get; set; }
        public string CreatedByName { get; set; }
    }
}
