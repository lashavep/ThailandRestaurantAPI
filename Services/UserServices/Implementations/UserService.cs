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


        public async Task<IEnumerable<UserDTO>> GetAllAsync()
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
    });
}
        public async Task<UserDTO?> GetByEmailAsync(string email)
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
            };
        }

        public async Task<UserDTO?> GetByIdAsync(int id)
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
            };
        }

        public Task<UserDTO> RegisterAsync(User user)
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
            });
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.FirstName = dto.FirstName ?? user.FirstName;
                user.LastName = dto.LastName ?? user.LastName;
                user.Phone = dto.Phone ?? user.Phone;
                user.Address = dto.Address ?? user.Address;
                user.Zipcode = dto.Zipcode ?? user.Zipcode;
                user.Gender = dto.Gender ?? user.Gender;
                user.Age = dto.Age ?? user.Age;
                user.IsSubscribedToPromo = dto.IsSubscribedToPromo;
            }
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<UserDTO> DeleteUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException("User not found");

            if (user.Role == "Admin")
                throw new InvalidOperationException("Cannot delete an Admin user");

            var deleteUser = _userRepository.DeleteUserById(id);
            return new UserDTO
            {
                Id = deleteUser.Result.Id,
                Email = deleteUser.Result.Email,
                FirstName = deleteUser.Result.FirstName,
                LastName = deleteUser.Result.LastName,
                Age = deleteUser.Result.Age,
                Address = deleteUser.Result.Address,
                Phone = deleteUser.Result.Phone,
                Zipcode = deleteUser.Result.Zipcode,
                IsSubscribedToPromo = deleteUser.Result.IsSubscribedToPromo
            };
        }
    }
}
