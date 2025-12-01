using RestaurantAPI.DTOs.OrderDTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.OrderServices.Interfaces
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(CreateOrderDTO dto, int userId);
        Task<IEnumerable<OrderResponseDTO>> GetOrdersByUserAsync(int userId);
        Task<(bool Success, string Message, Order? Order)> AcceptOrderAsync(int id);
        Task<(bool Success, string Message, Order? Order)> RejectOrderAsync(int id);
        Task<object?> GetOrderDetailsAsync(int id);
        Task<object> GetOrdersByStatusAsync(string status, int page, int pageSize);
    }

}
