using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs.BasketDTO;
using RestaurantAPI.Services.BasketServices.Interfaces;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
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

    // GET /api/baskets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BasketDto>>> GetAll()
    {
        int userId = GetUserId();
        var baskets = await _basketService.GetAllAsync(userId);
        return Ok(baskets);
    }

    // POST /api/baskets/items
    [HttpPost("addToBasket")]
    public async Task<ActionResult<BasketDto>> AddToBasket([FromBody] BasketPostDto dto)
    {
        dto.UserId = GetUserId();
        var result = await _basketService.AddToBasketAsync(dto);
        return Ok(result);
    }

    // PUT /api/baskets/items/{itemId}
    [HttpPut("updateBasket/{itemId}")]
    public async Task<IActionResult> UpdateBasket(int ItemId, [FromBody] UpdateBasketDto dto)
    {
        dto.UserId = GetUserId();
        dto.ItemId = ItemId;
        var success = await _basketService.UpdateBasketAsync(dto);
        if (!success) return NotFound("Basket item not found");
        return NoContent();
    }

    // DELETE /api/baskets/items/{productId}
    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        int userId = GetUserId();
        var success = await _basketService.DeleteProductAsync(productId, userId);
        if (!success) return NotFound($"Product with ID {productId} not found in basket");
        return NoContent();
    }

    // DELETE /api/baskets
    [HttpDelete("ClearBasket")]
    public async Task<IActionResult> ClearBasket()
    {
        int userId = GetUserId();
        await _basketService.ClearBasketAsync(userId);
        return NoContent();
    }
}
