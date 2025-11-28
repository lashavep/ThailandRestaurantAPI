namespace RestaurantAPI.DTOs.OrderDTO
{
    public class CreateOrderDTO
    {
        public int UserId { get; set; }
        public List<OrderItemDTO> Items { get; set; }
        public decimal Total { get; set; }
        public string? Address { get; set; }
    }

    public class OrderItemDTO
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
