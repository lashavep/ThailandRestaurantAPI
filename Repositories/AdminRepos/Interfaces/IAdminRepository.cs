using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.AdminRepos.Interfaces
{
    public interface IAdminRepository
    {
       
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Category?> GetCategoryByNameAsync(string name);
        Task<Category?> GetCategoryByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task<Product?> GetProductWithCategoryAsync(int id);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task DeleteUserByIdAsync(int userId);
        Task PromoteUserAsync(int userId, string newRole);
    }
}
