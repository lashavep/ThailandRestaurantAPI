using RestaurantAPI.DTOs.IngredientDTO;
using RestaurantAPI.DTOs.ProductDTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.ProductServices.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();                                                                        // ყველა პროდუქტის მიღება
        Task<IEnumerable<ProductDto>> GetFilteredAsync(bool? vegeterian, bool? nuts, int? spiciness, int? categoryId);      // ფილტრირებული პროდუქტის მიღება
        Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync();                                                          // ყველა ინგრედიენტის მიღება
        Task<List<IngredientDto>> GetIngredientByProductIdAsync(int productId);                                             // პროდუქტის მიხედვით ინგრედიენტების მიღება
        Task<ProductIngredient> AddIngredientAsync(int productId, string name);                                             // ინგრედიენტის დამატება პროდუქტისთვის
        Task<ProductIngredient> UpdateIngredientAsync(int id, string name);                                                 // ინგრედიენტის განახლება პროდუქტისთვის
        Task<bool> DeleteIngredientAsync(int id);                                                                           // ინგრედიენტის წაშლა პროდუქტისთვის

    }
}
