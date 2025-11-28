namespace RestaurantAPI.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public User? User { get; set; }
        public int UserId { get; set; }
    }

}
