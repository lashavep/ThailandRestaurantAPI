using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.CategoryRepos.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddAsync(Category category); 
        Task<bool> DeleteAsync(int id);
    }
}
