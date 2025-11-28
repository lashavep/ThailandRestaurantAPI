using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.ProductRepos.Interfaces;

namespace RestaurantAPI.Repositories.ProductRepos.Implementatios
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Ingredients)
                .ToListAsync();
        }

        public async Task AddIngredientAsync(ProductIngredient ingredient)
        {
            _context.ProductIngredients.Add(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateIngredientAsync(ProductIngredient ingredient)
        {
            _context.ProductIngredients.Update(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteIngredientAsync(ProductIngredient ingredient)
        {
            _context.ProductIngredients.Remove(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductIngredient?> GetIngredientByIdAsync(int id)
        {
            return await _context.ProductIngredients.FindAsync(id);
        }

    }
}
