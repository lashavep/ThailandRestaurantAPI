using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.ProductRepos.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();                           // ყველა პროდუქტის მიღება
        Task AddIngredientAsync(ProductIngredient ingredient);              // ინგრედიენტის დამატება პროდუქტისთვის
        Task UpdateIngredientAsync(ProductIngredient ingredient);           // ინგრედიენტის განახლება პროდუქტისთვის
        Task DeleteIngredientAsync(ProductIngredient ingredient);           // ინგრედიენტის წაშლა პროდუქტისთვის
        Task<ProductIngredient?> GetIngredientByIdAsync(int id);            // ინგრედიენტის მიღება ID-ის მიხედვით

    }
}
