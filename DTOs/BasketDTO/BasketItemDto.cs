using RestaurantAPI.DTOs.ProductDTO;

namespace RestaurantAPI.DTOs.BasketDTO
{
    public class BasketItemDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public ProductDto Product { get; set; }

        public int Quantity { get; set; }
        public double Price { get; set; }   // snapshot price per unit
    }
}
