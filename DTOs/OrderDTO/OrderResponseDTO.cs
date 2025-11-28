namespace RestaurantAPI.DTOs.OrderDTO
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public List<OrderItemDTO> Items { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
    }

}
