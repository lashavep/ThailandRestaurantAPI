using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.AuthDTO
{
    public class ForgotPasswordDTO                          // DTO პაროლის აღდგენის მოთხოვნისთვის
    {
        [Required]                                          
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
