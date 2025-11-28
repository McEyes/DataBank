using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Emails
{
    public class EmailMessage
    {
        public IEnumerable<string> MailTo { get; set; }
        public IEnumerable<string> MailCC { get; set; }
        public IEnumerable<string> MailBCC { get; set; }
        public string Subject { get; set; }
        public string Html { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string Account { get; set; }
        public string Pwd { get; set; }
        public List<Attachment> Attachments { get; set; }
        public List<string> AttachmentList { get; set; }
        public List<string> ImgList { get; set; }
        public string MailFooter { get; set; }
        public string AttachmentName { get; set; }
        public Stream StreamAttachment { get; set; }
    }
}
