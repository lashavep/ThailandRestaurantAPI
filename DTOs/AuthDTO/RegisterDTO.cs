using System.ComponentModel.DataAnnotations;
using static RestaurantAPI.Models.User;

namespace RestaurantAPI.DTOs.AuthDTO
{
    public class RegisterDTO
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        [EmailAddress] public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Zipcode { get; set; }
        public GenderType Gender { get; set; }
        public bool IsSubscribedToPromo { get; set; }
    }
}
