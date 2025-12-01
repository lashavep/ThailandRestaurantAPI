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


            string htmlBody = $@"
<!DOCTYPE html>
<html lang='ka'>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1'>
  <style>
    body {{
      margin:0;
      background:#f4f6f9;
      font-family:'Segoe UI', Arial, sans-serif;
      color:#333;
    }}
    .container {{
      max-width:640px;
      margin:40px auto;
      background:#d2691e;
      border-radius:12px;
      overflow:hidden;
      box-shadow:0 6px 24px rgba(0,0,0,.08);
    }}
    .header {{
      background:#111827;
      color:#d2691e;
      padding:20px;
      font-size:20px;
      font-weight:600;
      text-align:center;
    }}
    .content {{
      padding:24px;
      line-height:1.6;
    }}
    .content h2 {{
      color:#111827;
      margin-top:0;
    }}
    .content p {{
      color:#374151;
      font-size:15px;
    }}
    .btn {{
      display:inline-block;
      margin-top:20px;
      background:#d2691e;
      color:#fff;
      padding:12px 20px;
      border-radius:8px;
      text-decoration:none;
      font-weight:500;
    }}
    .footer {{
      background:#f9fafb;
      color:#6b7280;
      font-size:12px;
      padding:16px;
      text-align:center;
      border-top:1px solid #e5e7eb;
    }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>RiverSide Food Lab</div>
    <div class='content'>
      <h2>{dto.Subject}</h2>
      <p>{dto.Body}</p>
      <a href='http://localhost:4200/' class='btn'>Go to website
</a>
    </div>
    <div class='footer'>
      This message was sent from our service.<br/>
If you do not wish to receive promotional emails, you can unsubscribe from your profile.

    </div>
  </div>
</body>
</html>";


            foreach (var user in users.Where(u => u.IsSubscribedToPromo))
            {
                await _emailService.SendEmailAsync(user.Email, dto.Subject, htmlBody);
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
