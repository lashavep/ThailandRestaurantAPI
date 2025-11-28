using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.OrderRepos.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Task<List<Order>> GetByUserIdAsync(int userId);
    }

}
