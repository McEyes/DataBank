using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ITPortal.Flow.Application.EmailSendRecord.Dtos
{
    [SugarTable("EmailSendRecord")]
    public partial class EmailSendRecordEntity : AuditEntity<Guid>, ICreateNameEntity
    {
        [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public override Guid Id { get; set; }
        public Guid? FlowInstId { get; set; }
        public Guid? ActionId { get; set; }

        public Guid? EmailTempId { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        [SugarColumn(ColumnName = "Bcc")]
        public string EmailBcc { get; set; }
        public string EmailSubject { get; set; }
        public string EmailHtmlBody { get; set; }

        public int? RetryTimes { get; set; } = 3;
        /// <summary>
        /// 发送成功与失败：1成功，0没发送，-1失败，-99结束发送
        /// </summary>
        public int? Status { get; set; } = 0;
        public string ErrorMsg { get; set; }
        public DateTimeOffset? SendTime { get; set; }
        public string CreatedByName { get; set; }
    }
}
