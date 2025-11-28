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
        public async Task<ActionResult<IEnumerable<IngredientDto>>> GetIngredients()
        {
            var result = await _productService.GetAllIngredientsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> GetIngredientByProductId(int id)
        {
            var ingredients = await _dbcontext.ProductIngredients
                .Where(i => i.ProductId == id)
                .Select(i => new IngredientDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Ingredients = i.Ingredients,
                    ProductId = i.ProductId
                })
                .ToListAsync();

            if (!ingredients.Any())
                return NotFound($"No ingredient found for product with ID {id}");

            return Ok(ingredients);
        }


        [HttpPost("Add")]
        public async Task<IActionResult> AddIngredient(int productId, [FromBody] string name)
        {
            var result = await _productService.AddIngredientAsync(productId, name);
            return Ok(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] string name)
        {
            var result = await _productService.UpdateIngredientAsync(id, name);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var success = await _productService.DeleteIngredientAsync(id);
            if (!success) return NotFound($"Ingredient with ID {id} not found");
            return Ok($"Ingredient with ID {id} deleted successfully");
        }
    }
}
