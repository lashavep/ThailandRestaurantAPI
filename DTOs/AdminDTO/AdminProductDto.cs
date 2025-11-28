namespace RestaurantAPI.DTOs.AdminDTO
{
    public class AdminProductDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string? Image { get; set; }
        public int? Spiciness { get; set; }
        public bool? Vegeterian { get; set; }
        public bool? Nuts { get; set; }
        public string? CategoryName { get; set; }
        public List<string>? Ingredients { get; set; }

    }


}
