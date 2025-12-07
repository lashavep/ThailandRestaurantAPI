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

        public async Task SendEmailAsync(string to, string subject, string body)                            // Email-ის გაგზავნის მეთოდი
        {
            var host = _config["Smtp:Host"];                                                                // SMTP სერვერის ჰოსტი
            var port = int.Parse(_config["Smtp:Port"]);                                                     // SMTP სერვერის პორტი
            var user = _config["Smtp:User"];                                                                // SMTP მომხმარებელი (From address)
            var pass = _config["Smtp:Pass"];                                                                // SMTP პაროლი

            if (string.IsNullOrEmpty(user))                                                                 
                throw new InvalidOperationException("SMTP 'User' (From address) is not configured.");       // თუ მომხმარებელი არ არის კონფიგურირებული, გამოიტანოს შეტყობინება

            using (var client = new SmtpClient(host, port))                                                 // SmtpClient ობიექტის შექმნა
            {
                client.Credentials = new NetworkCredential(user, pass);                                     // ავტორიზაციის მონაცემების დაყენება
                client.EnableSsl = true;                                                                    // SSL-ის ჩართვა

                var mail = new MailMessage(user, to, subject, body)                                         // MailMessage ობიექტის შექმნა
                {
                    IsBodyHtml = true                                                                       //ვუთითებთ რომ body არის HTML
                };

                await client.SendMailAsync(mail);
            }
        }

    }


}
