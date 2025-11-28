using RestaurantAPI.DTOs.UserDTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.UserServices.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllAsync();
        Task<UserDTO?> GetByEmailAsync(string email);
        Task<UserDTO?> GetByIdAsync(int id);
        Task<UserDTO> RegisterAsync(User user);
        Task<UserDTO> DeleteUserById(int id);
        Task<bool> UpdateProfileAsync(int id, UpdateProfileDTO dto);
    }
}

