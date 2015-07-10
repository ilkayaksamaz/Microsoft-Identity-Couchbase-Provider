using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace INGA.Framework.EnterpriseLibraries.Authentication.Helpers
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            MailMessage mail = new MailMessage("ilkayaksamaz@gmail.com", message.Destination, message.Subject, message.Body)
            {
                IsBodyHtml = true
            };

            SmtpClient client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ilkayaksamaz@gmail.com", "karakartal1903")
            };
            return client.SendMailAsync(mail);
        }
    }
}
