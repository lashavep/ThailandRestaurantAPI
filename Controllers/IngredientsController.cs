using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs.IngredientDTO;
using RestaurantAPI.Models;
using RestaurantAPI.Services.ProductServices.Interfaces;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _dbcontext;

        public IngredientsController(IProductService productService, ApplicationDbContext _context)
        {
            _productService = productService;
            _dbcontext = _context;
        }

        [HttpGet("GetIngredients")]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> GetIngredients()                   // მეთოდი ყველა ინგრედიენტის მისაღებად
        {
            var result = await _productService.GetAllIngredientsAsync();                               // ინგრედიენტების მიღება სერვისიდან
            return Ok(result);                                                                         // HTTP 200 პასუხის დაბრუნება ინგრედიენტების სიით
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> GetIngredientByProductId(int id)   // მეთოდი ინგრედიენტების მისაღებად პროდუქტის ID-ის მიხედვით
        {
            var ingredients = await _dbcontext.ProductIngredients                                      // მიღება მონაცემთა ბაზიდან
                .Where(i => i.ProductId == id)                                                         // ფილტრაცია პროდუქტის ID-ის მიხედვით
                .Select(i => new IngredientDto                                                         // გადაკეთება IngredientDto ობიექტებად
                {
                    Id = i.Id,                                                                         //გადაცემული ინგრედიენტის ID
                    Name = i.Name,                                                                     //გადაცემული ინგრედიენტის სახელი
                    Ingredients = i.Ingredients,                                                       //გადაცემული ინგრედიენტის ინგრედიენტები
                    ProductId = i.ProductId                                                            //გადაცემული პროდუქტის ID
                })
                .ToListAsync();                                                                        // შედეგის სიად გადაქცევა

            if (!ingredients.Any())                                                                    // თუ არ არის ნაპოვნი ინგრედიენტები
                return NotFound($"No ingredient found for product with ID {id}");                      // დაბრუნება NotFound პასუხი

            return Ok(ingredients);                                                                    // დაბრუნება HTTP 200 პასუხი ინგრედიენტების სიით
        }


        [HttpPost("Add")]
        public async Task<IActionResult> AddIngredient(int productId, [FromBody] string name)          // მეთოდი ახალი ინგრედიენტის დამატებისთვის
        {
            var result = await _productService.AddIngredientAsync(productId, name);                    // ახალი ინგრედიენტის დამატება სერვისში
            return Ok(result);                                                                         // დაბრუნება HTTP 200 პასუხი დამატებული ინგრედიენტით
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] string name)               // მეთოდი ინგრედიენტის განახლებისთვის
        {
            var result = await _productService.UpdateIngredientAsync(id, name);                         // ინგრედიენტის განახლება სერვისში
            return Ok(result);                                                                          // დაბრუნება HTTP 200 პასუხი განახლებული ინგრედიენტით
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)                                       // მეთოდი ინგრედიენტის წასაშლელად
        {
            var success = await _productService.DeleteIngredientAsync(id);                              // ინგრედიენტის წაშლა სერვისში
            if (!success) return NotFound($"Ingredient with ID {id} not found");                        // თუ ინგრედიენტი არ არის ნაპოვნი, დაბრუნება NotFound პასუხი
            return Ok($"Ingredient with ID {id} deleted successfully");                                 // დაბრუნება HTTP 200 პასუხი წარმატების შეტყობინებით
        }
    }
}
