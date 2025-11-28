using RestaurantAPI.DTOs.AdminDTO;
using RestaurantAPI.DTOs.ProductDTO;

namespace RestaurantAPI.Services.AdminServices.Interfaces
{
    public interface IAdminService
    {
        Task<List<ProductWithCategoryDto>> GetAllProductsAsync();
        Task<ProductWithCategoryDto> CreateProductAsync(AdminProductDto dto);
        Task<ProductWithCategoryDto> UpdateProductAsync(int id, AdminProductDto dto);
        Task<bool> DeleteProductAsync(int id);
        Task PromoteUserAsync(int userId, string newRole);
        Task PromoteUserByEmailAsync(string email, string newRole);
        Task DeleteUserByIdAsync(int userId);
    }
}
