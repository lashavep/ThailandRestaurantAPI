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

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("GetFiltered")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetFiltered(
           bool? vegeterian,
           bool? nuts,
           int? spiciness,
           int? categoryId)
        {
            var products = await _productService.GetFilteredAsync(vegeterian, nuts, spiciness, categoryId);
            return Ok(products);
        }
    }
}
