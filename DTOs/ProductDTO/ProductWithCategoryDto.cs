namespace RestaurantAPI.DTOs.ProductDTO
{
    public class ProductWithCategoryDto                                     // DTO პროდუქტისთვის კატეგორიით
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Image { get; set; } = string.Empty;
        public int Spiciness { get; set; }
        public bool Vegeterian { get; set; }
        public bool Nuts { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<string> Ingredients { get; set; } = new();
    }
}
