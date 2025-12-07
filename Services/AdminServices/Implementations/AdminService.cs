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

            public async Task<List<ProductWithCategoryDto>> GetAllProductsAsync()         // ყველა პროდუქტის მიღება
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

            public async Task<ProductWithCategoryDto> CreateProductAsync(AdminProductDto dto)       // ახალი პროდუქტის შექმნა
            {
                var category = await _repo.GetCategoryByNameAsync(dto.CategoryName);                // კატეგორიის მიღება სახელის მიხედვით
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
                    
                    var ingredientsList = string.Join(", ", dto.Ingredients);       // ინგრედიენტების ჩამონათვალი ერთ სტრინგში

                    product.Ingredients.Add(new ProductIngredient                   // ახალი ProductIngredient ობიექტის შექმნა
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


            public async Task<ProductWithCategoryDto> UpdateProductAsync(int id, AdminProductDto dto)           // პროდუქტის განახლება
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

                
                if (dto.Price.HasValue) product.Price = (double)dto.Price.Value;                                // განახლება მხოლოდ მაშინ, თუ ახალი ფასი არის მოცემული
                if (dto.Vegeterian.HasValue) product.Vegeterian = dto.Vegeterian.Value;                         // განახლება მხოლოდ მაშინ, თუ ახალი მნიშვნელობა არის მოცემული
                if (dto.Nuts.HasValue) product.Nuts = dto.Nuts.Value;                                           // განახლება მხოლოდ მაშინ, თუ ახალი მნიშვნელობა არის მოცემული
                if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != "string") product.Name = dto.Name!;     // განახლება მხოლოდ მაშინ, თუ ახალი სახელი არ არის "string" ან ცარიელი
                if (!string.IsNullOrWhiteSpace(dto.Image) && dto.Image != "string") product.Image = dto.Image!; // განახლება მხოლოდ მაშინ, თუ ახალი სურათი არ არის "string" ან ცარიელი
                if (dto.Spiciness.HasValue) product.Spiciness = dto.Spiciness.Value;

               
                if (dto.Ingredients is not null && dto.Ingredients.Any())                                       // ინგრედიენტების განახლება
                {
                    var ingredientsList = string.Join(", ", dto.Ingredients);                                   // ინგრედიენტების ჩამონათვალი ერთ სტრინგში

                    product.Ingredients.Clear();                                                                // არსებული ინგრედიენტების წაშლა

                    product.Ingredients.Add(new ProductIngredient                                               // ახალი ProductIngredient ობიექტის შექმნა
                    {
                        ProductId = product.Id,
                        Name = product.Name,              
                        Ingredients = ingredientsList     
                    });
                }
                await _repo.UpdateProductAsync(product);                                                        // პროდუქტის განახლება რეპოზიტორიაში

                return new ProductWithCategoryDto                                                               // განახლებული პროდუქტის DTO-ის დაბრუნება
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
            public async Task<bool> DeleteProductAsync(int id)                                  // პროდუქტის წაშლა
            {
                var product = await _repo.GetProductByIdAsync(id);
                if (product == null) return false;

                await _repo.DeleteProductAsync(product);
                return true;
            }

            public async Task PromoteUserAsync(int userId, string newRole)                      // მომხმარებლის როლის განახლება
            {
                await _repo.PromoteUserAsync(userId, newRole);
            }

            public async Task PromoteUserByEmailAsync(string email, string newRole)             // მომხმარებლის როლის განახლება Email-ის მიხედვით
            {
                var user = await _repo.GetUserByEmailAsync(email);                              // მომხმარებლის მიღება Email-ის მიხედვით
                if (user == null)
                    throw new ArgumentException($"User with email '{email}' not found");        

                await _repo.PromoteUserAsync(user.Id, newRole);
            }

            public async Task DeleteUserByIdAsync(int userId)                                   // მომხმარებლის წაშლა ID-ის მიხედვით
            {
                await _repo.DeleteUserByIdAsync(userId);
            }
        }
    }
}
