using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.CategoryRepos.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();          // ყველა კატეგორიის მიღება
        Task<Category?> GetByIdAsync(int id);               // კატეგორიის მიღება ID-ის მიხედვით
        Task<Category> AddAsync(Category category);         // ახალი კატეგორიის დამატება
        Task<bool> DeleteAsync(int id);                     // კატეგორიის წაშლა ID-ის მიხედვით
    }
}
