using RestaurantAPI.DTOs.AdminDTO;
using RestaurantAPI.DTOs.ProductDTO;

namespace RestaurantAPI.Services.AdminServices.Interfaces
{
    public interface IAdminService
    {
        Task<List<ProductWithCategoryDto>> GetAllProductsAsync();                               // ყველა პროდუქტის მიღება
        Task<ProductWithCategoryDto> CreateProductAsync(AdminProductDto dto);                   // ახალი პროდუქტის შექმნა
        Task<ProductWithCategoryDto> UpdateProductAsync(int id, AdminProductDto dto);           // პროდუქტის განახლება
        Task<bool> DeleteProductAsync(int id);                                                  // პროდუქტის წაშლა
        Task PromoteUserAsync(int userId, string newRole);                                      // მომხმარებლის როლის განახლება
        Task PromoteUserByEmailAsync(string email, string newRole);                             // მომხმარებლის როლის განახლება Email-ის მიხედვით
        Task DeleteUserByIdAsync(int userId);                                                   // მომხმარებლის წაშლა ID-ის მიხედვით
    }
}
