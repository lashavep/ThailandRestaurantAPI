using RestaurantAPI.DTOs.CategoryDTO;
using RestaurantAPI.DTOs.ProductDTO;

namespace RestaurantAPI.Services.CategoryServices.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<GetCategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<GetCategoryDto> AddCategoryAsync(CreateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
