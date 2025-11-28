namespace RestaurantAPI.Services.AdminServices.Implementations
{
    using global::RestaurantAPI.Data;
    using global::RestaurantAPI.DTOs.AdminDTO;
    using global::RestaurantAPI.DTOs.ProductDTO;
    using global::RestaurantAPI.Models;
    using global::RestaurantAPI.Repositories.AdminRepos.Interfaces;
    using global::RestaurantAPI.Services.AdminServices.Interfaces;

    namespace RestaurantAPI.Services.AdminServices.Implementations
    {
        public class AdminService : IAdminService
        {
            private readonly IAdminRepository _repo;
            private readonly ApplicationDbContext _context;

            public AdminService(IAdminRepository repo, ApplicationDbContext context)
            {
                _repo = repo;
                _context = context;
            }

            public async Task<List<ProductWithCategoryDto>> GetAllProductsAsync()
            {
                var products = await _repo.GetAllProductsAsync();

                return products.Select(p => new ProductWithCategoryDto
                {
                    Id = p.Id,
                    Name = p.Name!,
                    Price = p.Price,
                    Image = p.Image!,
                    Spiciness = p.Spiciness,
                    Vegeterian = p.Vegeterian,
                    Nuts = p.Nuts,
                    CategoryName = p.Category?.Name ?? "Unknown"
                }).ToList();
            }

            public async Task<ProductWithCategoryDto> CreateProductAsync(AdminProductDto dto)
            {
                var category = await _repo.GetCategoryByNameAsync(dto.CategoryName);
                if (category == null) throw new ArgumentException("Invalid category name");

                var product = new Product
                {
                    Name = dto.Name,
                    Price = (double)dto.Price,
                    Image = dto.Image,
                    Spiciness = (int)dto.Spiciness,
                    Vegeterian = (bool)dto.Vegeterian,
                    Nuts = (bool)dto.Nuts,
                    CategoryId = category.Id,
                };

                if (dto.Ingredients is not null && dto.Ingredients.Any())
                {
                    
                    var ingredientsList = string.Join(", ", dto.Ingredients);

                    product.Ingredients.Add(new ProductIngredient
                    {
                        Name = dto.Name,                
                        Ingredients = ingredientsList,  
                        Product = product
                    });
                }

                await _repo.AddProductAsync(product);

                return new ProductWithCategoryDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Image = product.Image,
                    Spiciness = product.Spiciness,
                    Vegeterian = product.Vegeterian,
                    Nuts = product.Nuts,
                    CategoryName = category.Name
                };
            }


            public async Task<ProductWithCategoryDto> UpdateProductAsync(int id, AdminProductDto dto)
            {
                var product = await _repo.GetProductWithCategoryAsync(id);
                if (product == null) throw new KeyNotFoundException("Product not found");

                
                if (!string.IsNullOrWhiteSpace(dto.CategoryName) && dto.CategoryName != "string")
                {
                    var category = await _repo.GetCategoryByNameAsync(dto.CategoryName);
                    if (category == null)
                        throw new ArgumentException($"Category '{dto.CategoryName}' not found");

                    product.CategoryId = category.Id;
                    product.Category = category;
                }

                
                if (dto.Price.HasValue) product.Price = (double)dto.Price.Value;
                if (dto.Vegeterian.HasValue) product.Vegeterian = dto.Vegeterian.Value;
                if (dto.Nuts.HasValue) product.Nuts = dto.Nuts.Value;
                if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != "string") product.Name = dto.Name!;
                if (!string.IsNullOrWhiteSpace(dto.Image) && dto.Image != "string") product.Image = dto.Image!;
                if (dto.Spiciness.HasValue) product.Spiciness = dto.Spiciness.Value;

               
                if (dto.Ingredients is not null && dto.Ingredients.Any())
                {
                    var ingredientsList = string.Join(", ", dto.Ingredients);

                    product.Ingredients.Clear(); 

                    product.Ingredients.Add(new ProductIngredient
                    {
                        ProductId = product.Id,
                        Name = product.Name,              
                        Ingredients = ingredientsList     
                    });
                }
                await _repo.UpdateProductAsync(product);

                return new ProductWithCategoryDto
                {
                    Id = product.Id,
                    Name = product.Name!,
                    Price = product.Price,
                    Image = product.Image!,
                    Spiciness = product.Spiciness,
                    Vegeterian = product.Vegeterian,
                    Nuts = product.Nuts,
                    CategoryName = product.Category?.Name ?? "Unknown",
                    Ingredients = product.Ingredients
                        .SelectMany(i => i.Ingredients.Split(','))
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList()
                };
            }
            public async Task<bool> DeleteProductAsync(int id)
            {
                var product = await _repo.GetProductByIdAsync(id);
                if (product == null) return false;

                await _repo.DeleteProductAsync(product);
                return true;
            }

            public async Task PromoteUserAsync(int userId, string newRole)
            {
                await _repo.PromoteUserAsync(userId, newRole);
            }

            public async Task PromoteUserByEmailAsync(string email, string newRole)
            {
                var user = await _repo.GetUserByEmailAsync(email);
                if (user == null)
                    throw new ArgumentException($"User with email '{email}' not found");

                await _repo.PromoteUserAsync(user.Id, newRole);
            }

            public async Task DeleteUserByIdAsync(int userId)
            {
                await _repo.DeleteUserByIdAsync(userId);
            }
        }
    }
}
