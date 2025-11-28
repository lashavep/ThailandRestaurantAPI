using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs.CategoryDTO;
using RestaurantAPI.DTOs.ProductDTO;
using RestaurantAPI.Models;
using RestaurantAPI.Services.CategoryServices.Interfaces;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpPost("Add")]
        public async Task<ActionResult<GetCategoryDto>> Add([FromBody] CreateCategoryDto dto)
        {
            var category = await _categoryService.AddCategoryAsync(dto);
            return Ok(category);
        }



        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result) return NotFound($"Category with ID {id} not found");

            return Ok(new { message = $"Category {id} deleted" });
        }


        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<GetCategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("GetCategory/{id}")]
        
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound($"Category with ID {id} not found");

            return Ok(category);
        }
    }
}
