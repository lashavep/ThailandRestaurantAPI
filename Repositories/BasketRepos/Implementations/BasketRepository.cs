using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.BasketRepos.Interfaces;

namespace RestaurantAPI.Repositories.BasketRepos.Implementations
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ApplicationDbContext _db;
        public BasketRepository(ApplicationDbContext db) => _db = db;

        public async Task<Product?> GetProductByIdAsync(int productId)                                          // პროდუქტის მიღება ID-ის მიხედვით
            => await _db.Products.FindAsync(productId);

        public async Task<Basket?> GetBasketContainingProductAsync(int productId, int userId)                   // კალათის მიღება, რომელიც შეიცავს კონკრეტულ პროდუქტს მომხმარებლისთვის
            => await _db.Baskets.Include(b => b.Product)
                                .FirstOrDefaultAsync(b => b.ProductId == productId && b.UserId == userId);

        public async Task<IEnumerable<Basket>> GetAllByUserAsync(int userId)                                    // კალათში არსებული ყველა პროდუქტის მიღება მომხმარებლისთვის
            => await _db.Baskets.Include(b => b.Product)
                                .Where(b => b.UserId == userId)
                                .AsNoTracking()
                                .ToListAsync();

        public async Task<Basket> AddAsync(Basket basket)                                                       // ახალი კალათის შექმნა
        {
            _db.Baskets.Add(basket);
            await _db.SaveChangesAsync();
            await _db.Entry(basket).Reference(b => b.Product).LoadAsync();
            return basket;
        }

        public async Task UpdateAsync(Basket basket)                                                            // კალათის განახლება
        {
            _db.Baskets.Update(basket);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int productId, int userId)                                                // პროდუქტის წაშლა კალათიდან მომხმარებლისთვის
        {
            var existing = await _db.Baskets.FirstOrDefaultAsync(b => b.ProductId == productId && b.UserId == userId); 
            if (existing != null)
            {
                _db.Baskets.Remove(existing);
                await _db.SaveChangesAsync();
            }
        }
    }
}
