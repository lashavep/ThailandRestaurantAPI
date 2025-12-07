using Microsoft.AspNetCore.Mvc;

namespace RestaurantAPI.DTOs.ProductDTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Nuts { get; set; }
        public double Price { get; set; }
        public int Spiciness { get; set; }
        public bool Vegeterian { get; set; }

        public List<string> Ingredients { get; set; } = new();
    }
}
