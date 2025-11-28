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
        public async Task<IActionResult> Signup(RegisterDTO dto)
        {
            try
            {
                var result = await _auth.RegisterAsync(dto);
                return Ok(new { token = result.Token });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Something went wrong" });
            }
        }


        [HttpPost("sign_in")]
        public async Task<IActionResult> Signin(LoginDTO dto)
        {
            try
            {
                var result = await _auth.LoginAsync(dto);
                return Ok(new { token = result.Token, name = result.Name });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }

        [HttpPost("forgot_password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            try
            {
                await _auth.ForgotPasswordAsync(dto);
                return Ok(new { message = "Reset code sent to email" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Something went wrong" });
            }
        }

        [HttpPost("reset_password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(new { message = string.Join("; ", errors) });
            }

            var success = await _auth.ResetPasswordAsync(dto);
            if (!success)
                return BadRequest(new { message = "Invalid or expired code" });

            return Ok(new { message = "Password reset successful" });
        }
    }

}
