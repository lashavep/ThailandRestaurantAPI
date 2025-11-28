using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.AuthDTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
