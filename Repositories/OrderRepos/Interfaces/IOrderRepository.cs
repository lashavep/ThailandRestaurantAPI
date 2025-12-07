using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.OrderRepos.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);                          // ახალი შეკვეთის დამატება
        Task<List<Order>> GetByUserIdAsync(int userId);             //  მომხმარებლის ID-ის მიხედვით შეკვეთების მიღება
    }

}
