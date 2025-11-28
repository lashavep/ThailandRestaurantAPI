namespace RestaurantAPI.DTOs.OrderDTO
{
    public class OrderItemDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
