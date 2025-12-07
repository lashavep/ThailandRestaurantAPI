using static RestaurantAPI.Models.User;

namespace RestaurantAPI.DTOs.UserDTO
{
    public class UpdateProfileDTO                                                   // DTO პროფილის განახლებისთვის
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Zipcode { get; set; }
        public GenderType? Gender { get; set; }
        public bool IsSubscribedToPromo { get; set; }
        public int? Age { get; set; }
    }
}
