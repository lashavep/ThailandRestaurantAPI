namespace RestaurantAPI.DTOs.AuthDTO
{
    public class AuthResponseDTO                            // DTO ავტორიზაციის პასუხისთვის
    {
        public string Token { get; set; } = null!;          // JWT ტოკენი
        public string Name { get; set; } = null!;
        public int ExpiresIn { get; set; }                  // Token-ის ვადის გასვლის დრო წამებში
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
    }
}
