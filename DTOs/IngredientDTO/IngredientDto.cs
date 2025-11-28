namespace RestaurantAPI.DTOs.IngredientDTO
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public int ProductId { get; set; }
    }
}
