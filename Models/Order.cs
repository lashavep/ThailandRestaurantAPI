namespace RestaurantAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ItemsJson { get; set; }
        public decimal Total { get; set; }
        public string? Address { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime Date { get; set; } = DateTime.Now;
        public User User { get; set; }
    }

}
