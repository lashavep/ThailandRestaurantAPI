using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.BasketRepos.Interfaces
{
    public interface IBasketRepository
    {
        Task<Product?> GetProductByIdAsync(int productId);
        Task<Basket?> GetBasketContainingProductAsync(int productId, int userId);
        Task<Basket> AddAsync(Basket basket);
        Task UpdateAsync(Basket basket);
        Task DeleteAsync(int productId, int userId);
        Task<IEnumerable<Basket>> GetAllByUserAsync(int userId);
    }

}
