namespace RestaurantAPI.DTOs.AuthDTO
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int ExpiresIn { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
    }
}
