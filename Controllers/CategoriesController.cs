// CategoriesController
// პასუხისმგებელია კატეგორიების CRUD ოპერაციებზე.
// CategoryService.GetCategory() აბრუნებს ყველა კატეგორიას.
// CategoryService.AddCategoryAsync() ამატებს ახალ კატეგორიას.
// CategoryService.DeleteCategoryAsync() შლის კატეგორიას.

using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs.CategoryDTO;
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


        [HttpPost("AddCategory")]
        public async Task<ActionResult<GetCategoryDto>> Add([FromBody] CreateCategoryDto dto)           // მეთოდი ახალი კატეგორიის დამატებისთვის
        {
            var category = await _categoryService.AddCategoryAsync(dto);                                // ახალი კატეგორიის დამატება სერვისში
            return Ok(category);                                                                        // დაბრუნება HTTP 200 პასუხი დამატებული კატეგორიით
        }



        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> Delete(int id)                                                 // მეთოდი კატეგორიის წასაშლელად
        {
            var result = await _categoryService.DeleteCategoryAsync(id);                                // კატეგორიის წაშლა სერვისში
            if (!result) return NotFound($"Category with ID {id} not found");                           // თუ კატეგორია არ არსებობს, დაბრუნება NotFound პასუხი

            return Ok(new { message = $"Category {id} deleted" });                                      // დაბრუნება HTTP 200 პასუხი წარმატების შეტყობინებით
        }


        [HttpGet("GetAllCategory")]
        public async Task<ActionResult<IEnumerable<GetCategoryDto>>> GetAll()                           // მეთოდი ყველა კატეგორიის მისაღებად
        {
            var categories = await _categoryService.GetAllAsync();                                      // კატეგორიების მიღება სერვისიდან
            return Ok(categories);                                                                      // HTTP 200 პასუხის დაბრუნება კატეგორიების სიით
        }

        [HttpGet("GetCategory/{id}")]
        
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)                                // მეთოდი კატეგორიის მისაღებად ID-ის მიხედვით
        {
            var category = await _categoryService.GetByIdAsync(id);                                     // კატეგორიის მიღება სერვისიდან ID-ის მიხედვით
            if (category == null)
                return NotFound($"Category with ID {id} not found");                                    // თუ კატეგორია არ არსებობს, დაბრუნება NotFound პასუხი

            return Ok(category);                                                                        // HTTP 200 პასუხის დაბრუნება კატეგორიით
        }   
    }
}
