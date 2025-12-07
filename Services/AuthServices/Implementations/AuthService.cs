using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs.AuthDTO;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.UserRepos.Interfaces;
using RestaurantAPI.Services.AuthServices.Interfaces;
using RestaurantAPI.Services.EmailService.Interfaces;

namespace RestaurantAPI.Services.AuthServices.Implementations
{
        public class AuthService : IAuthService
        {
            private readonly IUserRepository _userRepo;
            private readonly ApplicationDbContext _db;
            private readonly IConfiguration _config;
            private readonly PasswordHasher<User> _passwordHasher;
            private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepo, IConfiguration config, ApplicationDbContext _dbContext, IEmailService emailService)
        {
            _userRepo = userRepo;
            _config = config;
            _db = _dbContext;
            _passwordHasher = new PasswordHasher<User>();
            _emailService = emailService;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)                             // რეგისტრაცია
        {
            if (dto.Password != dto.ConfirmPassword)                                                  // პაროლების შესაბამისობის შემოწმება
                throw new InvalidOperationException("Passwords do not match");                        // თუ პაროლები არ ემთხვევა, გამოიტანოს შეცდომა

            var emailNorm = dto.Email.Trim().ToLowerInvariant();                                      // ელფოსტის ნორმალიზაცია
            var phoneNorm = dto.Phone.Trim();                                                         // ტელეფონის ნომრის ნორმალიზაცია

            var existingEmail = await _userRepo.GetByEmailAsync(emailNorm);                           // ელფოსტის არსებობის შემოწმება
            if (existingEmail != null)
                throw new InvalidOperationException("Email already registered");                      // თუ ელფოსტა უკვე რეგისტრირებულია, გამოიტანოს შეტყობინება

            var existingPhone = await _userRepo.GetByPhoneAsync(phoneNorm);                             // ტელეფონის ნომრის არსებობის შემოწმება
            if (existingPhone != null)
                throw new InvalidOperationException("Phone number already registered");                 // თუ ტელეფონის ნომერი უკვე რეგისტრირებულია, გამოიტანოს შეტყობინება

            var user = new User                                                                         // ახალი მომხმარებლის ობიექტის შექმნა
            {
                Email = emailNorm,
                Phone = phoneNorm,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Age = dto.Age,
                Address = dto.Address,
                Zipcode = dto.Zipcode,
                Gender = (User.GenderType)dto.Gender,
                IsSubscribedToPromo = dto.IsSubscribedToPromo,
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);                       // პაროლის ჰეშირება

            var saved = await _userRepo.AddAsync(user);                                                 // მომხმარებლის შენახვა 
            var token = GenerateJwtToken(saved);                                                        // JWT ტოკენის გენერაცია
            var expiresIn = int.Parse(_config["Jwt:ExpiresInSeconds"] ?? "3600");                       // ტოკენის ვადის განსაზღვრა

            return new AuthResponseDTO                                                                  
            {
                Token = token,
                ExpiresIn = expiresIn,
                UserId = saved.Id,
                Email = saved.Email
            };
        }


        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)                                              //ავტორიზაცია
            {
                var emailNorm = dto.Email.Trim().ToLowerInvariant();                                              
                var user = await _userRepo.GetByEmailAsync(emailNorm);
                if (user == null)
                    throw new UnauthorizedAccessException("Invalid credentials");
                    
                var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);       // პაროლის შემოწმება
            if (verify == PasswordVerificationResult.Failed)                                                    // თუ პაროლი არ ემთხვევა
                throw new UnauthorizedAccessException("Invalid credentials");                                   // გამოიტანოს შეცდომა

            var token = GenerateJwtToken(user);                                                                 // JWT ტოკენის გენერაცია
            var expiresIn = int.Parse(_config["Jwt:ExpiresInSeconds"] ?? "3600");                               // ტოკენის ვადის განსაზღვრა

            return new AuthResponseDTO              
                {
                    Token = token,
                    ExpiresIn = expiresIn,
                    Name = user.FirstName = string.Empty,
                    UserId = user.Id,
                    Email = user.Email
                };
            }

            private string GenerateJwtToken(User user)                                                                                  // JWT ტოკენის გენერაცია
        {
                var keyBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not set"));    // საიდუმლო გასაღების მიღება კონფიგურაციიდან
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);                      // ხელმოწერის კრედენციების შექმნა

            var claims = new List<Claim>                                                                                                // კლეიმების შექმნა ტოკენისთვის
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),                                                             // Subject კლეიმი მომხმარებლის ID-სთვის
                new Claim(JwtRegisteredClaimNames.Email, user.Email),                                                                   // Email კლეიმი მომხმარებლის ელფოსტისთვის
                new Claim("fname", user.FirstName ?? string.Empty),                                                                     // FirstName კლეიმი
                new Claim(ClaimTypes.Role, user.Role),                                                                                  // Role კლეიმი
                new Claim("role", user.Role)                                                                                          
            };

                var expires = DateTime.UtcNow.AddSeconds(int.Parse(_config["Jwt:ExpiresInSeconds"] ?? "3600"));                         // ტოკენის ვადის განსაზღვრა

            var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );                                                                                                                      // JWT ტოკენის შექმნა

            return new JwtSecurityTokenHandler().WriteToken(token);                                                                     // ტოკენის სტრინგად გადაქცევა და დაბრუნება
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO dto)                                                                    // პაროლის აღდგენა
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email.ToLower());                                                            // მომხმარებლის მიღება ელფოსტის მიხედვით
            if (user == null)
                throw new InvalidOperationException("User not found");                                                                  // თუ მომხმარებელი არ არსებობს, გამოიტანოს შეტყობინება

            var code = new Random().Next(100000, 999999).ToString();                                                                    // 6-ნიშნა კოდის გენერაცია
            user.ResetToken = code;                                                                                                     // რეზეტ კოდის შენახვა მომხმარებლის ობიექტში
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(3);                                                                      // რეზეტ კოდის ვადის განსაზღვრა (3 წუთი)

            await _db.SaveChangesAsync();

            
            await _emailService.SendEmailAsync(user.Email, "Password Reset Code",                               // პაროლის აღდგენის კოდის გაგზავნა ელფოსტაზე
                $"Your password reset code is {code}. It expires in 3 minutes.");                               // ელფოსტის შინაარსი
        }



        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO dto)                                        // პაროლის შეცვლა
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email.ToLower());                                    // მომხმარებლის მიღება ელფოსტის მიხედვით
            if (user == null || user.ResetToken != dto.Token || user.ResetTokenExpiry < DateTime.UtcNow)        // რეზეტ კოდის და ვადის შემოწმება
                return false;

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);                            // ახალი პაროლის ჰეშირება და შენახვა
            user.ResetToken = null;                                                                             // რეზეტ კოდის წაშლა
            user.ResetTokenExpiry = null;                                                                       // რეზეტ კოდის და ვადის წაშლა

            await _db.SaveChangesAsync();
            return true;                                                                                        
        }

    }
}
