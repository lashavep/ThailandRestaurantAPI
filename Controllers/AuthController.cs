// AuthController
// პასუხისმგებელია ავტორიზაციაზე და რეგისტრაციაზე.
// AuthService.RegisterAsync() ქმნის ახალ მომხმარებელს.
// AuthService.LoginAsync() ამოწმებს მონაცემებს და აბრუნებს JWT ტოკენს.
// AuthService.ResetPassword() აგზავნის პაროლის აღდგენის იმეილს.

using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs.AuthDTO;
using RestaurantAPI.Services.AuthServices.Interfaces;

namespace RestaurantAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;


        [HttpPost("sign_up")]
        public async Task<IActionResult> Signup(RegisterDTO dto)                            // რეგისტრაციის მეთოდი
        {
            try
            {
                var result = await _auth.RegisterAsync(dto);                                // რეგისტრაციის სერვისის გამოძახება
                return Ok(new { token = result.Token });                                    // წარმატების პასუხი ტოკენით
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });                              // თუ მომხმარებელი უკვე არსებობს, დაბრუნება Conflict პასუხი
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Something went wrong" });           // ზოგადი შეცდომის პასუხი
            }
        }


        [HttpPost("sign_in")]
        public async Task<IActionResult> Signin(LoginDTO dto)                               // შესვლის მეთოდი
        {
            try
            {
                var result = await _auth.LoginAsync(dto);                                   // შესვლის სერვისის გამოძახება
                return Ok(new { token = result.Token, name = result.Name });                // წარმატების პასუხი ტოკენით და სახელით
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials" });               // თუ მონაცემები არასწორია, დაბრუნება Unauthorized პასუხი
            }
        }

        [HttpPost("forgot_password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)   // პაროლის აღდგენის მეთოდი
        {
            try
            {
                await _auth.ForgotPasswordAsync(dto);                                       // პაროლის აღდგენის სერვისის გამოძახება
                return Ok(new { message = "Reset code sent to email" });                    // წარმატების პასუხი
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });                              // თუ მომხმარებელი არ არსებობს, დაბრუნება NotFound პასუხი
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Something went wrong" });           // ზოგადი შეცდომის პასუხი
            }
        }

        [HttpPost("reset_password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)     // პაროლის შეცვლის მეთოდი
        {
            if (!ModelState.IsValid)                                                        // მოდელის ვალიდაციის შემოწმება
            {
                var errors = ModelState.Values                                              // შეცდომების შეგროვება
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);                                           // შეცდომების მესიჯების არჩევა
                return BadRequest(new { message = string.Join("; ", errors) });             // დაბრუნება BadRequest პასუხი შეცდომებით
            }

            var success = await _auth.ResetPasswordAsync(dto);                              // პაროლის შეცვლის სერვისის გამოძახება
            if (!success)
                return BadRequest(new { message = "Invalid or expired code" });             // თუ კოდი არასწორია ან ვადაგასულია, დაბრუნება BadRequest პასუხი

            return Ok(new { message = "Password reset successful" });                       // წარმატების პასუხი
        }
    }

}
