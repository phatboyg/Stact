using System.Collections.Generic;
using System.Net.Mail;

namespace Stact.Mail
{
    public interface IMailServer
    {
        void Send(MailMessage mailMessage);
        void Send(IList<MailMessage> mailMessages);
        void Send(string from, string to, string subject, string message);
    }
}