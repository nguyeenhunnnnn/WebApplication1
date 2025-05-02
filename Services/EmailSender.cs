using System.Net.Mail;
using System.Net;

namespace WebApplication1.Services
{
    public interface ICustomEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
    public class EmailSender : ICustomEmailSender
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly string _smtpUser = "hunoconnect@gmail.com";
        private readonly string _smtpPass = "ysjs jwln sqon pvxt";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var client = new SmtpClient(_smtpServer)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                Port = 587,
                EnableSsl = true
            };

            var mailMessage = new MailMessage(_smtpUser, toEmail, subject, body);
            await client.SendMailAsync(mailMessage);
        }
    }

}
