using RestaurantAPI.DTOs.OrderDTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.OrderServices.Interfaces
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(CreateOrderDTO dto, int userId);                        // შეკვეთის განთავსება
        Task<IEnumerable<OrderResponseDTO>> GetOrdersByUserAsync(int userId);               // მომხმარებლის მიხედვით შეკვეთების მიღება
        Task<(bool Success, string Message, Order? Order)> AcceptOrderAsync(int id);        // შეკვეთის დადასტურება
        Task<(bool Success, string Message, Order? Order)> RejectOrderAsync(int id);        // შეკვეთის უარყოფა
        Task<object?> GetOrderDetailsAsync(int id);                                         // შეკვეთის დეტალების მიღება
        Task<object> GetOrdersByStatusAsync(string status, int page, int pageSize);         // სტატუსის მიხედვით შეკვეთების მიღება
    }

}
