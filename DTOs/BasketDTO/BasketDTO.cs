using RestaurantAPI.DTOs.ProductDTO;

namespace RestaurantAPI.DTOs.BasketDTO
{
    public class BasketDTO
    {
        public int Id { get; set; }              // DB id
        public int Quantity { get; set; }
        public double Price { get; set; }
        public int ProductId { get; set; }
        public ProductDto Product { get; set; } = new ProductDto();
    }
}
