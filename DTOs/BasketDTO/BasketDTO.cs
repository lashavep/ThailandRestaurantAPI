using RestaurantAPI.DTOs.ProductDTO;

namespace RestaurantAPI.DTOs.BasketDTO
{
    public class BasketDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<BasketItemDto> Items { get; set; } = new();
    }
}
