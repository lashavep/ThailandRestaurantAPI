using System;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.ProductIngredientServices
{
    public class IngredientService
    {
        private readonly ApplicationDbContext _context;
        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductIngredient>> GetAllIngredientsAsync()       // ყველა ინგრედიენტის მიღება
        {
            var ingredients = await _context.ProductIngredients.ToListAsync();           // DB-დან ყველა ინგრედიენტის მიღება

            return ingredients.Select(i => new ProductIngredient                         // DTO-ში გადაყვანა
            {
                Id = i.Id,
                Name = i.Name,
                Ingredients = i.Ingredients
            });
        }
    }
}
