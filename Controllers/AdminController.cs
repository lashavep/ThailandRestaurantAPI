using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs.AdminDTO;
using RestaurantAPI.Services.AdminServices.Interfaces;
using RestaurantAPI.Services.UserServices.Interfaces;
using RestaurantAPI.Services.EmailService.Interfaces;

namespace RestaurantAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AdminController(IAdminService service, IUserService userService, IEmailService emailService)
        {
            _service = service;
            _userService = userService;
            _emailService = emailService;
        }

        // === Email ===
        [HttpPost("SendPromoEmail")]
        public async Task<IActionResult> SendPromoEmail([FromBody] PromoMessageDto dto)
        {
            var users = await _userService.GetAllAsync();

            foreach (var user in users.Where(u => u.IsSubscribedToPromo))
            {
                await _emailService.SendEmailAsync(user.Email, dto.Subject, dto.Body);
            }

            return Ok(new { message = "Promo emails sent successfully" });
        }

        [AllowAnonymous]
        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromBody] ContactMessageDto dto)
        {
            var subject = $"New contact form message from {dto.Name}";
            var body = $"Sender: {dto.Email}\n\nMessage:\n{dto.Message}";

            await _emailService.SendEmailAsync("foodlab.rs@gmail.com", subject, body);

            return Ok(new { message = "Message sent to admin" });
        }

        // === Products ===
        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _service.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] AdminProductDto dto)
        {
            var result = await _service.CreateProductAsync(dto);
            return Ok(result);
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] AdminProductDto dto)
        {
            var result = await _service.UpdateProductAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _service.DeleteProductAsync(id);
            if (result)
                return Ok($"The product with ID {id} has been successfully deleted!");
            else
                return NotFound($"The product with ID {id} not found!");
        }

        // === Users ===
        [HttpPut("PromoteUserByEmail")]
        public async Task<IActionResult> PromoteUserByEmail([FromQuery] string email, [FromQuery] string newRole)
        {
            await _service.PromoteUserByEmailAsync(email, newRole);
            return Ok($"User {email} promoted to {newRole}");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await _service.DeleteUserByIdAsync(userId);
            return Ok($"User {userId} deleted successfully");
        }
    }
}
