using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.AuthDTO
{
    public class ResetPasswordDTO                                   // DTO პაროლის შეცვლისთვის
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Token { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}
