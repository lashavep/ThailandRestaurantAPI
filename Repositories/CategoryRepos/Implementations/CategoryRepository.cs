using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.CategoryRepos.Interfaces;

namespace RestaurantAPI.Repositories.CategoryRepos.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> AddAsync(Category category)                                      // ახალი კატეგორიის დამატება
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)                                                 // კატეგორიის წაშლა ID-ის მიხედვით
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Category>> GetAllAsync()                                      // ყველა კატეგორიის მიღება
        {
            return await _context.Categories
            .Include(c => c.Products)
            .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)                                           // კატეგორიის მიღება ID-ის მიხედვით
        {
            return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
