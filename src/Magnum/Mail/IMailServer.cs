using System.Collections.Generic;
using System.Net.Mail;

namespace Magnum.Common.Mail
{
    public interface IMailServer
    {
        void Send(MailMessage mailMessage);
        void Send(IList<MailMessage> mailMessages);
        void Send(string from, string to, string subject, string message);
    }
}