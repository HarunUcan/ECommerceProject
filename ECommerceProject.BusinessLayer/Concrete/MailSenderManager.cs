using ECommerceProject.BusinessLayer.Abstract;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Concrete
{
    public class MailSenderManager : IMailSenderService
    {
        private readonly IConfiguration _configuration;

        public MailSenderManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendConfirmationMailAsync(string mailTo, string subject, string mailBody)
        {
            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailFrom = new MailboxAddress("ECommerceProject", "harunucan3330@gmail.com");
            MailboxAddress mailToAddress = new MailboxAddress("User", mailTo);

            mimeMessage.From.Add(mailFrom);
            mimeMessage.To.Add(mailToAddress);
            mimeMessage.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = mailBody;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("harunucan3330@gmail.com", "xugh qtqo oktf oons");
            await client.SendAsync(mimeMessage);
            client.Disconnect(true);

        }
    }
}
