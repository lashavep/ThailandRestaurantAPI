using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs.OrderDTO;
using RestaurantAPI.Services.OrderServices.Interfaces;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("placeOrder")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO dto)
    {
        var userId = GetUserId();
        var order = await _orderService.PlaceOrderAsync(dto, userId);
        return Ok(order);
    }

    [HttpPost("acceptOrder/{id}")]
    public async Task<IActionResult> AcceptOrder(int id)
    {
        var result = await _orderService.AcceptOrderAsync(id);
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message, order = result.Order });
    }

    [HttpPost("rejectOrder/{id}")]
    public async Task<IActionResult> RejectOrder(int id)
    {
        var result = await _orderService.RejectOrderAsync(id);
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message, order = result.Order });
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> GetOrderDetails(int id)
    {
        var result = await _orderService.GetOrderDetailsAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _orderService.GetAllOrdersAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetUserId();
        var result = await _orderService.GetOrdersByUserAsync(userId);
        return Ok(result);
    }

    private int GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out int userId)
            ? userId
            : throw new UnauthorizedAccessException("Invalid user ID");
    }
}
