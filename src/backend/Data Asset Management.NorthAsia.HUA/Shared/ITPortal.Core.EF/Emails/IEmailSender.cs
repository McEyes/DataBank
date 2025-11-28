using Furion.DependencyInjection;

namespace ITPortal.Core.Emails
{
    public interface IEmailSender : ISingleton
    {
        Task SendAsync(string[] mailTo, string[] mailCC, string subject, string body, string fromName = null);
        Task SendAsync(EmailMessage emailMessage);
    }
}
