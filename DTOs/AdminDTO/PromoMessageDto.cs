namespace RestaurantAPI.DTOs.AdminDTO
{
    public class PromoMessageDto                         // DTO სარეკლამო შეტყობინებისთვის ადმინისტრატორის მიერ     
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
