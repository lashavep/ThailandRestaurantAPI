using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs.IngredientDTO;
using RestaurantAPI.DTOs.ProductDTO;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.ProductRepos.Interfaces;
using RestaurantAPI.Services.ProductServices.Interfaces;

namespace RestaurantAPI.Services.ProductServices.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ApplicationDbContext _context;


        public ProductService(IProductRepository repository, ApplicationDbContext dbContext)
        {
            _repository = repository;
            _context = dbContext;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Nuts = p.Nuts,
                Image = p.Image,
                Vegeterian = p.Vegeterian,
                Spiciness = p.Spiciness,
                CategoryId = p.CategoryId,
                Ingredients = p.Ingredients
                    .SelectMany(i => i.Ingredients.Split(','))
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList()
            });
        }

        public async Task<IEnumerable<ProductDto>> GetFilteredAsync(bool? vegeterian, bool? nuts, int? spiciness, int? categoryId)
        {
            var products = await _repository.GetAllAsync();

            var filtered = products.Where(p =>
                (!vegeterian.HasValue || p.Vegeterian == vegeterian.Value) &&
                (!nuts.HasValue || p.Nuts == nuts.Value) &&
                (!spiciness.HasValue || p.Spiciness == spiciness.Value) &&
                (!categoryId.HasValue || p.CategoryId == categoryId.Value)
            );

            return filtered.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Nuts = p.Nuts,
                Image = p.Image,
                Vegeterian = p.Vegeterian,
                Spiciness = p.Spiciness,
                CategoryId = p.CategoryId
            });
        }

        public async Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync()
        {
            var ingredients = await _context.ProductIngredients.ToListAsync();

            return ingredients.Select(i => new IngredientDto
            {
                Id = i.Id,
                Name = i.Name,
                Ingredients = i.Ingredients,
                ProductId = i.ProductId
            });
        }


        public async Task<List<IngredientDto>> GetIngredientByProductIdAsync(int productId)
        {
            var ingredients = await _context.ProductIngredients
                .Where(i => i.ProductId == productId)
                .Select(i => new IngredientDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Ingredients = i.Ingredients
                })
                .ToListAsync();

            return ingredients;
        }


        public async Task<ProductIngredient> AddIngredientAsync(int productId, string ingredientName)
        {
            var ingredient = new ProductIngredient
            {
                ProductId = productId,
                Name = ingredientName,
                Ingredients = ingredientName,
            };

            await _repository.AddIngredientAsync(ingredient);
            return ingredient;
        }

        public async Task<ProductIngredient> UpdateIngredientAsync(int id, string name)
        {
            var ingredient = await _repository.GetIngredientByIdAsync(id);
            if (ingredient == null) throw new KeyNotFoundException("Ingredient not found");

            ingredient.Name = name;
            ingredient.Ingredients = name;

            await _repository.UpdateIngredientAsync(ingredient);
            return ingredient;
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            var ingredient = await _repository.GetIngredientByIdAsync(id);
            if (ingredient == null) return false;

            await _repository.DeleteIngredientAsync(ingredient);
            return true;
        }

    }
}

