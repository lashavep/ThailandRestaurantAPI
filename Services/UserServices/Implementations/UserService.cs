using RestaurantAPI.DTOs.UserDTO;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.UserRepos.Interfaces;
using RestaurantAPI.Services.UserServices.Interfaces;

namespace RestaurantAPI.Services.UserServices.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) => _userRepository = userRepository;


        public async Task<IEnumerable<UserDTO>> GetAllAsync()                               // ყველა მომხმარებლის მიღება
        {
    var allUsers = await _userRepository.GetAllAsync();
    return allUsers.Select(user => new UserDTO
    {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Age = user.Age,
        Address = user.Address,
        Phone = user.Phone,
        Zipcode = user.Zipcode,
        Gender = user.Gender,
        CreatedAt = user.CreatedAt,
        IsSubscribedToPromo = user.IsSubscribedToPromo,
    });                                                                             // LINQ-ის Select მეთოდით ყველა მომხმარებლის DTO-ში გადასაყვანად
        }
        public async Task<UserDTO?> GetByEmailAsync(string email)                   // მომხმარებლის მიღება Email-ის მიხედვით
        {
            var userByEmail = await _userRepository.GetByEmailAsync(email);
            return userByEmail == null ? null : new UserDTO
            {
                Id = userByEmail.Id,
                Email = userByEmail.Email,
                FirstName = userByEmail.FirstName,
                LastName = userByEmail.LastName,
                Age = userByEmail.Age,
                Address = userByEmail.Address,
                Phone = userByEmail.Phone,
                Zipcode = userByEmail.Zipcode,
                IsSubscribedToPromo = userByEmail.IsSubscribedToPromo,
            };                                                                      // მომხმარებლის DTO-ში გადაყვანა, თუ მომხმარებელი არსებობს
        }

        public async Task<UserDTO?> GetByIdAsync(int id)                            // მომხმარებლის მიღება ID-ის მიხედვით
        {
            var userById = await _userRepository.GetByIdAsync(id);
            return userById == null ? null : new UserDTO
            {
                Id = userById.Id,
                Email = userById.Email,
                FirstName = userById.FirstName,
                LastName = userById.LastName,
                Age = userById.Age,
                Address = userById.Address,
                Phone = userById.Phone,
                Zipcode = userById.Zipcode,
                Gender = userById.Gender,
                CreatedAt = userById.CreatedAt,
                IsSubscribedToPromo = userById.IsSubscribedToPromo,
            };                                                                      // მომხმარებლის DTO-ში გადაყვანა, თუ მომხმარებელი არსებობს
        }

        public Task<UserDTO> RegisterAsync(User user)                               // ახალი მომხმარებლის რეგისტრაცია
        {
            var addedUser = _userRepository.AddAsync(user);
            return Task.FromResult(new UserDTO
            {
                Id = addedUser.Result.Id,
                Email = addedUser.Result.Email,
                FirstName = addedUser.Result.FirstName,
                LastName = addedUser.Result.LastName,
                Age = addedUser.Result.Age,
                Address = addedUser.Result.Address,
                Phone = addedUser.Result.Phone,
                Zipcode = addedUser.Result.Zipcode,
                IsSubscribedToPromo = addedUser.Result.IsSubscribedToPromo,
                Gender = addedUser.Result.Gender,
            });                                                                     // დამატებული მომხმარებლის DTO-ში გადაყვანა
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileDTO dto)    // მომხმარებლის პროფილის განახლება
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");


            var emailNorm = dto.Email?.Trim().ToLowerInvariant();
            var phoneNorm = dto.Phone?.Trim();

            // Email uniqueness check
            if (!string.IsNullOrEmpty(emailNorm))
            {
                var existingEmail = await _userRepository.GetByEmailAsync(emailNorm);
                if (existingEmail != null && existingEmail.Id != user.Id)
                    throw new InvalidOperationException("Email already registered");
            }

            // Phone uniqueness check
            if (!string.IsNullOrEmpty(phoneNorm))
            {
                var existingPhone = await _userRepository.GetByPhoneAsync(phoneNorm);
                if (existingPhone != null && existingPhone.Id != user.Id)
                    throw new InvalidOperationException("Phone number already registered");
            }

            // განახლება DTO-ის მიხედვით
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Phone = phoneNorm ?? user.Phone;
            user.Address = dto.Address ?? user.Address;
            user.Zipcode = dto.Zipcode ?? user.Zipcode;
            user.Gender = dto.Gender ?? user.Gender;
            user.Age = dto.Age ?? user.Age;
            user.Email = emailNorm ?? user.Email;
            user.IsSubscribedToPromo = dto.IsSubscribedToPromo;

            return await _userRepository.UpdateAsync(user);
        }


        public async Task<bool> DeleteUserById(int id)                              // მომხმარებლის წაშლა ID-ის მიხედვით
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException("User not found");

            if (user.Role == "Admin")
                throw new InvalidOperationException("Cannot delete an Admin user");

            await _userRepository.DeleteUserById(id);
            return true;
        }
    }
}
