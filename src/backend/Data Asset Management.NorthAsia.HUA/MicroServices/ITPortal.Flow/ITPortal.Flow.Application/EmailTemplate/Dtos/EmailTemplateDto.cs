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
    public partial class EmailTemplateDto:PageEntity<Guid>
    {
        public string EmailName { get; set; }
        public string EmailTitle { get; set; }

        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }
        public string EmailSubject { get; set; }
        public string EmailTemplate { get; set; }


        public int? Sort { get; set; }
        public bool? IsEnabled { get; set; }
        public string Remark { get; set; }
        public string CreatedByName { get; set; }
    }
}
