using RestaurantAPI.DTOs.BasketDTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.BasketServices.Interfaces
{
    public interface IBasketService
    {
        Task<IEnumerable<BasketDTO>> GetAllAsync(int userId);
        Task<bool> UpdateBasketAsync(UpdateBasketDto dto);
        Task<BasketDTO> AddToBasketAsync(BasketPostDto dto);
        Task<bool> DeleteProductAsync(int productId, int userId);
        Task ClearBasketAsync(int userId);
    }
}
