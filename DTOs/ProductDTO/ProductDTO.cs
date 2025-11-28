using Microsoft.AspNetCore.Mvc;

namespace RestaurantAPI.DTOs.ProductDTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public bool Nuts { get; set; }
        public string? Image { get; set; }
        public bool Vegeterian { get; set; }
        public int Spiciness { get; set; }
        public int CategoryId { get; set; }
        public List<string> Ingredients { get; set; } = new();
    }
}
