using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.AuthDTO
{
    public class LoginDTO
    {
        [Required][EmailAddress] public string Email { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
    }
}
