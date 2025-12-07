using RestaurantAPI.DTOs.CategoryDTO;
using RestaurantAPI.DTOs.ProductDTO;

namespace RestaurantAPI.Services.CategoryServices.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<GetCategoryDto>> GetAllAsync();                        // ყველა კატეგორიის მიღება
        Task<CategoryDto?> GetByIdAsync(int id);                                // კატეგორიის მიღება ID-ის მიხედვით
        Task<GetCategoryDto> AddCategoryAsync(CreateCategoryDto dto);           // ახალი კატეგორიის დამატება
        Task<bool> DeleteCategoryAsync(int id);                                 // კატეგორიის წაშლა ID-ის მიხედვით
    }
}
