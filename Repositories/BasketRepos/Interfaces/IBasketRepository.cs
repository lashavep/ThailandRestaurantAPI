using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.BasketRepos.Interfaces
{
    public interface IBasketRepository
    {
        Task<Product?> GetProductByIdAsync(int productId);                              // პროდუქტის მიღება ID-ის მიხედვით
        Task<Basket?> GetBasketContainingProductAsync(int productId, int userId);       // კალათის მიღება, რომელიც შეიცავს კონკრეტულ პროდუქტს მომხმარებლისთვის
        Task<Basket> AddAsync(Basket basket);                                           // ახალი კალათის დამატება
        Task UpdateAsync(Basket basket);                                                // კალათის განახლება
        Task DeleteAsync(int productId, int userId);                                    // პროდუქტის წაშლა კალათიდან მომხმარებლისთვის
        Task<IEnumerable<Basket>> GetAllByUserAsync(int userId);                        // ყველა კალათის მიღება მომხმარებლისთვის
    }

}
