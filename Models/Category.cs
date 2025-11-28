using RestaurantAPI.DTOs.ProductDTO;

namespace RestaurantAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<Product>? Products { get; set; }
    }

}
