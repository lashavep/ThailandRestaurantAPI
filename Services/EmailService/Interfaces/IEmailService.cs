namespace RestaurantAPI.Services.EmailService.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);            
    }

}
