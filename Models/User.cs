namespace RestaurantAPI.Models
{
    public class User
    {
        public enum GenderType
        {
            Male = 0,
            Female = 1,
            Other = 2
        }
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "User";
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Zipcode { get; set; }
        public GenderType Gender { get; set; }
        public bool IsSubscribedToPromo { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
