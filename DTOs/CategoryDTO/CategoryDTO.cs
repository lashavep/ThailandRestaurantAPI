using RestaurantAPI.DTOs.ProductDTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.DTOs.CategoryDTO
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Product>? Products { get; set; }
    }

}
