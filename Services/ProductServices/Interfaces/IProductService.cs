using RestaurantAPI.DTOs.IngredientDTO;
using RestaurantAPI.DTOs.ProductDTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.ProductServices.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<IEnumerable<ProductDto>> GetFilteredAsync(bool? vegeterian, bool? nuts, int? spiciness, int? categoryId);
        Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync();
        Task<List<IngredientDto>> GetIngredientByProductIdAsync(int productId);
        Task<ProductIngredient> AddIngredientAsync(int productId, string name);
        Task<ProductIngredient> UpdateIngredientAsync(int id, string name);
        Task<bool> DeleteIngredientAsync(int id);

    }
}
