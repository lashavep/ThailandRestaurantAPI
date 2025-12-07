namespace RestaurantAPI.DTOs.AdminDTO
{
    public class ContactMessageDto                          // DTO კონტაქტური ფორმის შეტყობინებისთვის
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
