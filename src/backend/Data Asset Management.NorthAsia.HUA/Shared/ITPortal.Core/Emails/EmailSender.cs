using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace ITPortal.Core.Emails
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;
        private readonly SmtpServerConfig _smtpServer;
        public EmailSender(
            IConfiguration configuration,
            ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _smtpServer = new SmtpServerConfig();

            _smtpServer.HostName = _configuration.GetValue<string>("SmtpServer:Default:HostName");
            _smtpServer.Port = _configuration.GetValue("SmtpServer:Default:Port", 25);
            _smtpServer.FromAddress = _configuration.GetValue<string>("SmtpServer:Default:FromAddress");
            _smtpServer.FromName = _configuration.GetValue<string>("SmtpServer:Default:FromName");
            _smtpServer.Account = _configuration.GetValue<string>("SmtpServer:Default:Account");
            _smtpServer.PassWord = _configuration.GetValue<string>("SmtpServer:Default:PassWord");
            _smtpServer.TestEmailAddress = _configuration.GetValue<string>("SmtpServer:Default:TestEmailAddress");
            _smtpServer.Enabled = _configuration.GetValue<bool>("SmtpServer:Default:Enabled");

        }

        public async Task SendAsync(string[] mailTo, string[] mailCC, string subject, string body, string fromName = null)
        {
            EmailMessage emailMessage = new EmailMessage();

            emailMessage.MailTo = mailTo;
            emailMessage.MailCC = mailCC;
            emailMessage.Subject = subject;
            emailMessage.Html = body;
            emailMessage.FromName = fromName;

            await SendAsync(emailMessage);
        }

        public Task SendAsync(EmailMessage emailMessage)
        {
            if (!_smtpServer.Enabled)
                return Task.CompletedTask;

            if (emailMessage.MailTo == null || emailMessage.MailTo.Count() == 0)
                return Task.CompletedTask;

            if (string.IsNullOrWhiteSpace(emailMessage.FromAddress)) emailMessage.FromAddress = _smtpServer.FromAddress;
            if (string.IsNullOrWhiteSpace(emailMessage.FromName)) emailMessage.FromName = _smtpServer.FromName;
            if (string.IsNullOrWhiteSpace(emailMessage.Account)) emailMessage.Account = _smtpServer.Account;
            if (string.IsNullOrWhiteSpace(emailMessage.Pwd)) emailMessage.Pwd = _smtpServer.PassWord;

            if (!string.IsNullOrWhiteSpace(_smtpServer.TestEmailAddress))
            {
                emailMessage.MailTo = _smtpServer.TestEmailAddress.Split(',');
                emailMessage.MailCC = null;
            }

            MailAddress sender = new MailAddress(emailMessage.FromAddress, emailMessage.FromName);
            MailMessage mail = new MailMessage();
            mail.To.Clear();
            try
            {
                AddMailTo(emailMessage.MailTo, mail);

                AddMailCC(emailMessage.MailCC, mail);

                AddMailBCC(emailMessage.MailBCC, mail);

                SmtpClient _smtpClient = new SmtpClient();
                _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//Specified the mode of send email
                _smtpClient.Host = _smtpServer.HostName;
                _smtpClient.Port = _smtpServer.Port;
                _smtpClient.Credentials = new System.Net.NetworkCredential(emailMessage.Account, emailMessage.Pwd);


                mail.From = sender;
                mail.Sender = sender;
                mail.Subject = emailMessage.Subject;
                mail.Body = emailMessage.Html;
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                if (emailMessage.AttachmentList != null && emailMessage.AttachmentList.Count > 0)
                {
                    mail.Attachments.Clear();
                    for (int j = 0; j < emailMessage.AttachmentList.Count; j++)
                    {
                        Attachment ath = new Attachment(emailMessage.AttachmentList[j]);
                        mail.Attachments.Add(ath);
                    }
                }

                if (emailMessage.ImgList != null && emailMessage.ImgList.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string img in emailMessage.ImgList)
                    {
                        Attachment att = new Attachment(img);
                        sb.Append(string.Format("<br><img src=\"cid:{0}\"/>", att.ContentId));
                        mail.Attachments.Add(att);
                    }
                    mail.Body += sb.ToString();
                }

                if (!string.IsNullOrWhiteSpace(emailMessage.AttachmentName) && emailMessage.StreamAttachment != null)
                {
                    mail.Attachments.Clear();
                    Attachment ath = new Attachment(emailMessage.StreamAttachment, emailMessage.AttachmentName);
                    mail.Attachments.Add(ath);
                }
                if (emailMessage.Attachments != null && emailMessage.Attachments.Count > 0)
                {
                    mail.Attachments.Clear();
                    for (int j = 0; j < emailMessage.Attachments.Count; j++)
                    {
                        mail.Attachments.Add(emailMessage.Attachments[j]);
                    }
                }

                mail.Body += emailMessage.MailFooter;

                mail.Priority = MailPriority.Normal;

                _smtpClient.Send(mail);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                foreach (Attachment item in mail.Attachments) { item.Dispose(); }
            }

        }

        private static void AddMailCC(IEnumerable<string> mailCC, MailMessage mail)
        {
            AddMailAddress(mailCC, mail.CC);
        }

        private static void AddMailBCC(IEnumerable<string> mailBCC, MailMessage mail)
        {
            AddMailAddress(mailBCC, mail.Bcc);
        }

        private static void AddMailTo(IEnumerable<string> mailTo, MailMessage mail)
        {
            AddMailAddress(mailTo, mail.To);
        }


        private static void AddMailAddress(IEnumerable<string> mailAddresses, MailAddressCollection addresses)
        {
            if (mailAddresses == null || mailAddresses.Count() == 0) return;

            if (mailAddresses.Count() == 1)
            {
                addresses.Add(mailAddresses.ElementAt(0));
            }
            else
            {
                foreach (var strMailAddress in mailAddresses)
                {
                    string sMailAddress = strMailAddress.Trim();
                    if (!string.IsNullOrEmpty(sMailAddress) && !addresses.Contains(new MailAddress(sMailAddress)))
                    {
                        addresses.Add(sMailAddress);
                    }
                }
            }
        }
    }
}
