// ProductService.CreateProduct() ამატებს ახალ პროდუქტს.
// ProductService.UpdateProduct() ანახლებს პროდუქტს.
// ProductService.DeleteProduct() შლის პროდუქტს.
// ProductDto.CategoryName → პროდუქტის კატეგორიის სახელი.

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
        public async Task<IActionResult> SendPromoEmail([FromBody] PromoMessageDto dto)                         // მეთოდი სარეკლამო მეილების გასაგზავნად
        {
            var users = await _userService.GetAllAsync();                                                       // ყველა მომხმარებლის მიღება სერვისიდან


            string htmlBody = $@"                                                                               // HTML შაბლონი სარეკლამო მეილისთვის
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


            foreach (var user in users.Where(u => u.IsSubscribedToPromo))                                   // მხოლოდ იმ მომხმარებლებისთვის, რომლებიც გამოწერილნი არიან სარეკლამო მეილებზე
            {
                await _emailService.SendEmailAsync(user.Email, dto.Subject, htmlBody);                      // მეილის გაგზავნა EmailService-ის საშუალებით
            }

            return Ok(new { message = "Promo emails sent successfully" });                                  // წარმატების პასუხის დაბრუნება
        }   


        [AllowAnonymous]
        [HttpPost("SendMailToAdmin")]
        public async Task<IActionResult> Send([FromBody] ContactMessageDto dto)                             // მეთოდი კონტაქტური ფორმის შეტყობინების გაგზავნისთვის ადმინისტრატორთან
        {
            var subject = $"New contact form message from {dto.Name}";                                      // მეილის თემის შექმნა
            var body = $"Sender: {dto.Email}\n\nMessage:\n{dto.Message}";                                   // მეილის შინაარსის შექმნა

            await _emailService.SendEmailAsync("foodlab.rs@gmail.com", subject, body);                      // მეილის გაგზავნა ადმინისტრატორის იმეილზე

            return Ok(new { message = "Message sent to admin" });                                           // წარმატების პასუხის დაბრუნება
        }


        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProducts()                                                   // მეთოდი ადმინისტრატვორთან ყველა პროდუქტის მისაღებად
        { 
            var products = await _service.GetAllProductsAsync();                                            // პროდუქტების მიღება ადმინისტრატორის სერვისიდან
            return Ok(products);                                                                            // HTTP 200 პასუხის დაბრუნება პროდუქტების სიით
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] AdminProductDto dto)                      // მეთოდი ახალი პროდუქტის შექმნისთვის
        {
            var result = await _service.CreateProductAsync(dto);                                            // ახალი პროდუქტის შექმნა ადმინისტრატორის სერვისში
            return Ok(result);
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] AdminProductDto dto)              // მეთოდი პროდუქტის განახლებისთვის
        {
            var result = await _service.UpdateProductAsync(id, dto);                                        // პროდუქტის განახლება ადმინისტრატორის სერვისში
            return Ok(result);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)                                              //  მეთოდი პროდუქტის წასაშლელად
        {
            var result = await _service.DeleteProductAsync(id);                                             // პროდუქტის წაშლა ადმინისტრატორის სერვისში
            if (result)
                return Ok($"The product with ID {id} has been successfully deleted!");                      // წარმატების პასუხის დაბრუნება
            else
                return NotFound($"The product with ID {id} not found!");                                    // NotFound პასუხის დაბრუნება, თუ პროდუქტი არ არის ნაპოვნი
        }


        [HttpPut("PromoteUserByEmail")]
        public async Task<IActionResult> PromoteUserByEmail([FromQuery] string email, [FromQuery] string newRole)       // მეთოდი მომხმარებლის როლის შეცვლისთვის ელფოსტის მიხედვით
        {
            await _service.PromoteUserByEmailAsync(email, newRole);                                                     // მომხმარებლის როლის შეცვლა ადმინისტრატორის სერვისში
            return Ok($"User {email} promoted to {newRole}");                                                           // წარმატების პასუხის დაბრუნება
        }

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)                                                         // მეთოდი მომხმარებლის წასაშლელად ID-ის მიხედვით
        {
            await _service.DeleteUserByIdAsync(userId);                                                                 // მომხმარებლის წაშლა ადმინისტრატორის სერვისში
            return Ok($"User {userId} deleted successfully");                                                           // წარმატების პასუხის დაბრუნება
        }
    }
}
