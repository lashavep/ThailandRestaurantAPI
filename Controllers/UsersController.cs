// UsersController
// პასუხისმგებელია მომხმარებლის პროფილის მართვაზე.
// UserService.GetProfile() აბრუნებს მომხმარებლის მონაცემებს.
// UserService.UpdateProfile() ანახლებს პროფილს.
// UserService.DeleteUserById() შლის მომხმარებელს.


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs.UserDTO;
using RestaurantAPI.Models;
using RestaurantAPI.Services.UserServices.Interfaces;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _db;
        public UsersController(IUserService userService, ApplicationDbContext dbContext)
        {
            _userService = userService;
            _db = dbContext;
        }


        [HttpPost("UserRegister")]                                              // მომხმარებლის რეგისტრაცია.
        public async Task<IActionResult> RegisterUser([FromBody] User user)     // იღებს მომხმარებლის ობიექტს.
        {
            try
            {
                var createdUser = await _userService.RegisterAsync(user);       // ქმნის ახალ მომხმარებელს UserService-ის საშუალებით.
                return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser); // აბრუნებს CreatedAtAction პასუხს, რომელიც შეიცავს შექმნილ მომხმარებელს.
            }
            catch (InvalidOperationException ex)                                // თუ მომხმარებელი უკვე არსებობს, იჭერს ექსეფშენს.
            {
                return Conflict(ex.Message);                                    // აბრუნებს Conflict პასუხს შესაბამის შეტყობინებასთან ერთად.
            }
        }

        [Authorize(Roles = "Admin")]                                            // მხოლოდ ადმინისტრატორებისთვის.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()     // მეთოდი ყველა მომხმარებლის მისაღებად
        {
            var users = await _userService.GetAllAsync();                       // მომხმარებლების მიღება სერვისიდან
            return Ok(users);                                                   // HTTP 200 პასუხის დაბრუნება მომხმარებლების სიით.
        }

        [Authorize]
        [HttpGet("GetUserProfile")]                                             // მომხმარებლის პროფილის მიღება.
        public async Task<IActionResult> GetProfile()                           // მეთოდი ავტორიზებული მომხმარებლის პროფილის მისაღებად
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub); // მომხმარებლის ID-ს მიღება ტოკენიდან
            var user = await _userService.GetByIdAsync(int.Parse(userId));      // მომხმარებლის მონაცემების მიღება სერვისიდან ID-ის მიხედვით
            if (user == null)
                return NotFound();                                              // თუ მომხმარებელი არ არსებობს, აბრუნებს NotFound პასუხს

            return Ok(new                               
            {
                user.FirstName,
                user.LastName,
                user.Email,
                user.Phone,
                user.Age,
                user.Gender,
                user.Address,
                user.Zipcode,
                user.IsSubscribedToPromo
            });                                                                 // HTTP 200 პასუხის დაბრუნება მომხმარებლის მონაცემებით
        }



        [Authorize]
        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)   // მომხმარებლის პროფილის განახლება
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);                    // მომხმარებლის ID-ს მიღება ტოკენიდან
            if (!int.TryParse(userId, out int id))                                          // ID-ის გადაყვანა int ტიპში
                return Unauthorized();                                                      // თუ ID არ არის ვალიდური, აბრუნებს Unauthorized პასუხს

            try
            {
                var success = await _userService.UpdateProfileAsync(id, dto); 

                if (!success)
                    return BadRequest(new { message = "Update failed" });                   // თუ განახლება ვერ მოხერხდა, აბრუნებს BadRequest პასუხს

                return Ok(new { message = "Profile updated successfully" });                // HTTP 200 პასუხის დაბრუნება წარმატების შეტყობინებით
            }
            catch (InvalidOperationException ex)
            {
                
                return BadRequest(new { message = ex.Message });                            // თუ მომხმარებელი არ არსებობს, აბრუნებს BadRequest პასუხს შესაბამის შეტყობინებასთან ერთად
            }
            catch (Exception)
            {
               
                return StatusCode(500, new { message = "An unexpected error occurred" });   // ზოგადი შეცდომის პასუხი
            }
        }





        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)                           // მეთოდი მომხმარებლის მიღებისთვის ID-ის მიხედვით
        {
            var user = await _userService.GetByIdAsync(id);                             // მომხმარებლის მიღება სერვისიდან ID-ის მიხედვით
            if (user == null) return NotFound();                                        // თუ მომხმარებელი არ არსებობს, აბრუნებს NotFound პასუხს
                return Ok(user);                                                        // HTTP 200 პასუხის დაბრუნება მომხმარებლის მონაცემებით
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<User>> GetByEmail(string email)                  // მეთოდი მომხმარებლის მიღებისთვის ელფოსტის მიხედვით
        {
            var user = await _userService.GetByEmailAsync(email);                       
            if (user == null) return NotFound();                                        // თუ მომხმარებელი არ არსებობს, აბრუნებს NotFound პასუხს
            return Ok(user);                                                            // HTTP 200 პასუხის დაბრუნება მომხმარებლის მონაცემებით
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserById(int id)                         // მეთოდი მომხმარებლის წასაშლელად ID-ის მიხედვით
        {
            try
            {
                var deletedUser = await _userService.DeleteUserById(id);                // მომხმარებლის წაშლა სერვისში ID-ის მიხედვით
                return Ok($"User with id {id} successfully deleted");                   // HTTP 200 პასუხის დაბრუნება წარმატების შეტყობინებით
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);                                            // თუ მომხმარებელი არ არსებობს, აბრუნებს NotFound პასუხს შესაბამის შეტყობინებასთან ერთად
            }
        }
    }
}
