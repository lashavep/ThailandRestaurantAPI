// ProductsController
// ProductService.GetAllProduct() აბრუნებს პროდუქტების სიას.


using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs.ProductDTO;
using RestaurantAPI.Services.ProductServices.Interfaces;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetAllProduct")]                                          // როუტი ყველა პროდუქტის მისაღებად
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()   // მეთოდი ყველა პროდუქტის მისაღებად
        {
            var products = await _productService.GetAllAsync();             // პროდუქტების მიღება სერვისიდან
            return Ok(products);                                            // HTTP 200 პასუხის დაბრუნება პროდუქტების სიით
        }

        [HttpGet("GetFiltered")]                                            // როუტი ფილტრირებული პროდუქტების მისაღებად
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetFiltered(
           bool? vegeterian,
           bool? nuts,
           int? spiciness,
           int? categoryId)                                                 // მეთოდი ფილტრირებული პროდუქტების მისაღებად
        {
            var products = await _productService.GetFilteredAsync(vegeterian, nuts, spiciness, categoryId); // ფილტრირებული პროდუქტების მიღება სერვისიდან
            return Ok(products);                                            // HTTP 200 პასუხის დაბრუნება ფილტრირებული პროდუქტების სიით
        }
    }
}
