using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs.BasketDTO;
using RestaurantAPI.Services.BasketServices.Interfaces;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketsController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;

            throw new UnauthorizedAccessException("User ID is invalid or missing.");
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<BasketDTO>>> GetAll()
        {
            try
            {
                int userId = GetUserId();
                var baskets = await _basketService.GetAllAsync(userId);
                return Ok(baskets);
            }
            catch
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPost("AddToBasket")]
        public async Task<ActionResult<BasketDTO>> AddToBasket([FromBody] BasketPostDto dto)
        {
            try
            {
                dto.UserId = GetUserId();
                var result = await _basketService.AddToBasketAsync(dto);
                return Ok(result);
            }
            catch
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPut("UpdateBasket")]
        public async Task<IActionResult> UpdateBasket([FromBody] UpdateBasketDto dto)
        {
            try
            {
                dto.UserId = GetUserId();
                var success = await _basketService.UpdateBasketAsync(dto);
                if (!success) return NotFound("Basket item not found");
                return NoContent();
            }
            catch
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpDelete("DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                int userId = GetUserId();
                var success = await _basketService.DeleteProductAsync(productId, userId);
                if (!success) return NotFound($"Product with ID {productId} not found in basket");
                return NoContent();
            }
            catch
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpDelete("Clear")]
        public async Task<IActionResult> ClearBasket()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _basketService.ClearBasketAsync(int.Parse(userId));
            return NoContent();
        }

    }
}
