using RestaurantAPI.DTOs.CategoryDTO;
using RestaurantAPI.DTOs.ProductDTO;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.CategoryRepos.Interfaces;
using RestaurantAPI.Services.CategoryServices.Interfaces;

namespace RestaurantAPI.Services.CategoryServices.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }


        public async Task<GetCategoryDto> AddCategoryAsync(CreateCategoryDto dto)
        {
            var entity = new Category { Name = dto.Name };
            var saved = await _repository.AddAsync(entity);

            return new GetCategoryDto
            {
                Id = saved.Id,
                Name = saved.Name
            };
        }


        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<GetCategoryDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(c => new GetCategoryDto
            {
                Id = c.Id,
                Name = c.Name ?? string.Empty,
            });
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            CategoryDto? result = null;

            if (entity != null)
            {
                result = new CategoryDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Products = (entity.Products ?? new List<Product>()).Select(p => new Product
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Nuts = p.Nuts,
                        Image = p.Image,
                        Vegeterian = p.Vegeterian,
                        Spiciness = p.Spiciness,
                        CategoryId = p.CategoryId
                    }).ToList()
                };
            }
            return result;
        }

    }
}
