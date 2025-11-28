using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Emails
{
    public class SmtpServerConfig
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string Account { get; set; }
        public string PassWord { get; set; }
        public bool Enabled { get; set; }
        public string TestEmailAddress { get; set; }
    }
}
