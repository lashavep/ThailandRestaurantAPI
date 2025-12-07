// OrdersController
// პასუხისმგებელია შეკვეთების მართვაზე.
// OrderService.GetOrders() აბრუნებს მომხმარებლის შეკვეთებს.
// OrderService.PlaceOrder() ქმნის ახალ შეკვეთას.
// OrderService.AcceptOrder() ადასტურებს შეკვეთას.
// OrderService.RejectOrder() უარყოფს შეკვეთას.
// OrderService.GetOrdersByStatus() აბრუნებს შეკვეთებს სტატუსის მიხედვით.
// EmailService.SendOrderStatusEmail() აგზავნის შეტყობინებას მომხმარებელთან.

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs.OrderDTO;
using RestaurantAPI.Services.OrderServices.Interfaces;

[Authorize] // უზრუნველყოფს, რომ მხოლოდ ავტორიზებული მომხმარებლები შეძლებენ ამ კონტროლერის მეთოდების გამოყენებას.
[ApiController] 
[Route("api/orders")]
public class OrdersController : ControllerBase // OrdersController კლასი, რომელიც მართავს შეკვეთების API მოთხოვნებს.
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) // კონსტრუქტორი, რომელიც ინიცირებს IOrderService-ს.
    {
        _orderService = orderService;
    }

    [HttpPost("placeOrder")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO dto) // შეკვეთის შექმნა.
    {
        var userId = GetUserId();                                         // იღებს ავტორიზებული მომხმარებლის ID-ს JWT ტოკენიდან.
        var order = await _orderService.PlaceOrderAsync(dto, userId);      // ქმნის ახალ შეკვეთას OrderService-ის საშუალებით, გადაცემული DTO და userId-ის გამოყენებით.
        return Ok(order);                                                   // აბრუნებს შექმნილ შეკვეთას.
    }

    [HttpPost("acceptOrder/{id}")]
    public async Task<IActionResult> AcceptOrder(int id) // შეკვეთის მიღება იუზერის ID-ის მიხედვით.
    {
        var result = await _orderService.AcceptOrderAsync(id); // იღებს შეკვეთას OrderService-დან ID-ის მიხედვით.
        if (!result.Success)
            return BadRequest(new { message = result.Message }); // თუ მიღება ვერ მოხერხდა, აბრუნებს BadRequest.

        return Ok(new { message = result.Message, order = result.Order }); // აბრუნებს წარმატების შეტყობინებას და შეკვეთის დეტალებს.
    }

    [HttpPost("rejectOrder/{id}")]
    public async Task<IActionResult> RejectOrder(int id) // შეკვეთის უარყოფა ID-ის მიხედვით.
    {
        var result = await _orderService.RejectOrderAsync(id); // უარყოფს შეკვეთას OrderService-დან ID-ის მიხედვით.
        if (!result.Success)
            return BadRequest(new { message = result.Message }); // თუ უარყოფა ვერ მოხერხდა, აბრუნებს BadRequest.

        return Ok(new { message = result.Message, order = result.Order }); // აბრუნებს წარმატების შეტყობინებას და შეკვეთის დეტალებს.
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> GetOrderDetails(int id) // შეკვეთის დეტალების მიღება ID-ის მიხედვით.
    {
        var result = await _orderService.GetOrderDetailsAsync(id); // მოაქვს შეკვეთის დეტალები OrderService-დან ID-ის მიხედვით.
        if (result == null) // თუ შეკვეთა არ არსებობს, აბრუნებს NotFound.
            return NotFound();

        return Ok(result); // აბრუნებს შეკვეთის დეტალებს
    }

    [HttpGet("GetAllOrders")]
    public async Task<IActionResult> GetAllOrders([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)      // შეკვეთების ფილტრი სტატუსის მიხედვით (მაგ: "Pending", "Accepted", "Rejected").
    {
        var result = await _orderService.GetOrdersByStatusAsync(status, page, pageSize); // მოაქვს შეკვეთები სტატუსის მიხედვით OrderService-დან გვერდებად დაყოფილი.
        return Ok(result);         // აბრუნებს შეკვეთების სიას
    }

    
    [HttpGet("myOrders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetUserId();                                           // იღებს ავტორიზებული მომხმარებლის ID-ს JWT ტოკენიდან.
        var result = await _orderService.GetOrdersByUserAsync(userId);      // მოაქვს შეკვეთები კონკრეტული userId-ს მიხედვით OrderService-დან.
        return Ok(result);                                                  // აბრუნებს შეკვეთების სიას
    }

    //როცა მომხმარებელი აკეთებს შეკვეთას ან ცდილობს ნახოს თავისი შეკვეთები, სისტემამ ზუსტად უნდა იცოდეს რომელი მომხმარებელია.
    //GetUserId() მეთოდი უზრუნველყოფს, რომ ყოველი მოქმედება (მაგ: GetOrders(), CreateOrder()) შესრულდეს მიმდინარე ავტორიზებული მომხმარებლის ID‑ზე.
    private int GetUserId() // მეთოდი, რომელიც იღებს ავტორიზებული მომხმარებლის ID-ს JWT ტოკენიდან.
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);     // ეძებს NameIdentifier ტიპის კლეიმს მომხმარებლის ტოკენში.
        return int.TryParse(claim, out int userId)                      // კლაიმის მნიშვნელობა გადადის int ტიპში.
            ? userId                                                    // წარმატების შემთხვევაში აბრუნებს userId-ს.
            : throw new UnauthorizedAccessException("Invalid user ID"); // თუ კლეიმს არ არსებობს ან არ გადადის int-ში, იშვება UnauthorizedAccessException.
    }
}
