namespace RestaurantAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public bool Nuts { get; set; }
        public string? Image { get; set; }
        public bool Vegeterian { get; set; }
        public int Spiciness { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<ProductIngredient> Ingredients { get; set; } = new List<ProductIngredient>();
    }

}
