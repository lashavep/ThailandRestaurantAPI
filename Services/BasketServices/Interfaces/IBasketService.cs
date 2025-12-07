using RestaurantAPI.DTOs.BasketDTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.BasketServices.Interfaces
{
    public interface IBasketService
    {
        Task<BasketDto> GetAllAsync(int userId);               // ყველა პროდუქტის მიღება კალათიდან მომხმარებლისთვის
        Task<bool> UpdateBasketAsync(UpdateBasketDto dto);                  // კალათის პროდუქტის განახლება
        Task<BasketDto> AddToBasketAsync(BasketPostDto dto);                // პროდუქტის დამატება კალათაში
        Task<bool> DeleteProductAsync(int productId, int userId);           // პროდუქტის წაშლა კალათიდან მომხმარებლისთვის
        Task ClearBasketAsync(int userId);                                  // კალათის გასუფთავება მომხმარებლისთვის
    }
}
