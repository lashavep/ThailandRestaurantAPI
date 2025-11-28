using System.Net;
using System.Net.Mail;
using RestaurantAPI.Services.EmailService.Interfaces;

namespace RestaurantAPI.Services.EmailService.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var host = _config["Smtp:Host"];
            var port = int.Parse(_config["Smtp:Port"]);
            var user = _config["Smtp:User"];
            var pass = _config["Smtp:Pass"];

            if (string.IsNullOrEmpty(user))
                throw new InvalidOperationException("SMTP 'User' (From address) is not configured.");

            using (var client = new SmtpClient(host, port))
            {
                client.Credentials = new NetworkCredential(user, pass);
                client.EnableSsl = true;

                var mail = new MailMessage(user, to, subject, body);
                await client.SendMailAsync(mail);
            }
        }
    }


}
