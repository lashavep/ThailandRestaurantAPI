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

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new InvalidOperationException("Passwords do not match");

            var emailNorm = dto.Email.Trim().ToLowerInvariant();
            var phoneNorm = dto.Phone.Trim();

            var existingEmail = await _userRepo.GetByEmailAsync(emailNorm);
            if (existingEmail != null)
                throw new InvalidOperationException("Email already registered");

            var existingPhone = await _userRepo.GetByPhoneAsync(phoneNorm);
            if (existingPhone != null)
                throw new InvalidOperationException("Phone number already registered");

            var user = new User
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

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            var saved = await _userRepo.AddAsync(user);
            var token = GenerateJwtToken(saved);
            var expiresIn = int.Parse(_config["Jwt:ExpiresInSeconds"] ?? "3600");

            return new AuthResponseDTO
            {
                Token = token,
                ExpiresIn = expiresIn,
                UserId = saved.Id,
                Email = saved.Email
            };
        }


        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
            {
                var emailNorm = dto.Email.Trim().ToLowerInvariant();
                var user = await _userRepo.GetByEmailAsync(emailNorm);
                if (user == null)
                    throw new UnauthorizedAccessException("Invalid credentials");

                var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
                if (verify == PasswordVerificationResult.Failed)
                    throw new UnauthorizedAccessException("Invalid credentials");

                var token = GenerateJwtToken(user);
                var expiresIn = int.Parse(_config["Jwt:ExpiresInSeconds"] ?? "3600");

                return new AuthResponseDTO
                {
                    Token = token,
                    ExpiresIn = expiresIn,
                    Name = user.FirstName = string.Empty,
                    UserId = user.Id,
                    Email = user.Email
                };
            }

            private string GenerateJwtToken(User user)
            {
                var keyBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not set"));
                var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fname", user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("role", user.Role)
            };

                var expires = DateTime.UtcNow.AddSeconds(int.Parse(_config["Jwt:ExpiresInSeconds"] ?? "3600"));

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email.ToLower());
            if (user == null)
                throw new InvalidOperationException("User not found");

            var code = new Random().Next(100000, 999999).ToString();
            user.ResetToken = code;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(3);

            await _db.SaveChangesAsync();

            
            await _emailService.SendEmailAsync(user.Email, "Password Reset Code",
                $"Your password reset code is {code}. It expires in 3 minutes.");
        }



        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email.ToLower());
            if (user == null || user.ResetToken != dto.Token || user.ResetTokenExpiry < DateTime.UtcNow)
                return false;

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _db.SaveChangesAsync();
            return true;
        }

    }
}
