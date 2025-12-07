using System.ComponentModel.DataAnnotations;
using static RestaurantAPI.Models.User;

namespace RestaurantAPI.DTOs.UserDTO
{
    public class UserDTO                                                        // DTO მომხმარებლისთვის
    {
        public int Id { get; set; }
        [EmailAddress] public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Zipcode { get; set; }
        public GenderType Gender { get; set; }
        public bool IsSubscribedToPromo { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
