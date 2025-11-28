using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.UserRepos.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<bool> UpdateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByPhoneAsync(string phone);
        Task<User> AddAsync(User user);
        Task<User> DeleteUserById(int id);
    }
}
