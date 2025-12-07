using RestaurantAPI.DTOs.UserDTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services.UserServices.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllAsync();                           // ყველა მომხმარებლის მიღება
        Task<UserDTO?> GetByEmailAsync(string email);                       // მომხმარებლის მიღება Email-ის მიხედვით
        Task<UserDTO?> GetByIdAsync(int id);                                // მომხმარებლის მიღება ID-ის მიხედვით
        Task<UserDTO> RegisterAsync(User user);                             // ახალი მომხმარებლის რეგისტრაცია
        Task<bool> DeleteUserById(int id);                               // მომხმარებლის წაშლა ID-ის მიხედვით
        Task<bool> UpdateProfileAsync(int id, UpdateProfileDTO dto);        // მომხმარებლის პროფილის განახლება
    }
}

