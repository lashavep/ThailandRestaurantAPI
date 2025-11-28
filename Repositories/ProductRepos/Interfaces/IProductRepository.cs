using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.ProductRepos.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task AddIngredientAsync(ProductIngredient ingredient);
        Task UpdateIngredientAsync(ProductIngredient ingredient);
        Task DeleteIngredientAsync(ProductIngredient ingredient);
        Task<ProductIngredient?> GetIngredientByIdAsync(int id);

    }
}
