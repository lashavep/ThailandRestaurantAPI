using global::RestaurantAPI.Data;
using global::RestaurantAPI.Models;
using global::RestaurantAPI.Repositories.AdminRepos.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace RestaurantAPI.Repositories.AdminRepos.Implementations
{
    namespace RestaurantAPI.Repositories.AdminRepos.Implementations
    {
        public class AdminRepository : IAdminRepository
        {
            private readonly ApplicationDbContext _context;

            public AdminRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<Product>> GetAllProductsAsync()                                  // ყველა პროდუქტის მიღება
            {
                return await _context.Products.Include(p => p.Category).ToListAsync();              //დაბრუნება კატეგორიის მითითებით
            }

            public async Task<Product?> GetProductByIdAsync(int id)                                 // პროდუქტის მიღება ID-ის მიხედვით
            {
                return await _context.Products.FindAsync(id);                                       
            }

            public async Task<Category?> GetCategoryByNameAsync(string name)                        // კატეგორიის მიღება სახელის მიხედვით
            {
                return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);          
            }

            public async Task<Category?> GetCategoryByIdAsync(int id)                               // კატეგორიის მიღება ID-ის მიხედვით
            {
                return await _context.Categories.FindAsync(id);
            }

            public async Task<Product?> GetProductWithCategoryAsync(int id)                         // პროდუქტის მიღება კატეგორიის ID-ის მიხედვით
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Ingredients)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }

            public async Task AddProductAsync(Product product)                                      // ახალი პროდუქტის დამატება
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateProductAsync(Product product)                                   // პროდუქტის განახლება
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteProductAsync(Product product)                                   // პროდუქტის წაშლა
            {   
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            public async Task<User?> GetUserByIdAsync(int id)                                       // მომხმარებლის მიღება ID-ის მიხედვით
            {
                return await _context.Users.FindAsync(id);
            }

            public async Task<User?> GetUserByEmailAsync(string email)                              // მომხმარებლის მიღება Email-ის მიხედვით
            {   
                return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            }


            public async Task UpdateUserAsync(User user)                                            // მომხმარებლის განახლება
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteUserAsync(User user)                                            // მომხმარებლის წაშლა
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteUserByIdAsync(int userId)                                       // მომხმარებლის წაშლა ID-ის მიხედვით
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new KeyNotFoundException("User not found");                 

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            public async Task PromoteUserAsync(int userId, string newRole)                          // მომხმარებლის როლის განახლება
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new KeyNotFoundException("User not found");

                user.Role = newRole;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
    }

}
