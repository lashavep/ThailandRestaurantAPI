using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories.UserRepos.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();              // ყველა მომხმარებლის მიღება
        Task<bool> UpdateAsync(User user);                  // მომხმარებლის განახლება
        Task<User?> GetByEmailAsync(string email);          // მომხმარებლის მიღება Email-ის მიხედვით
        Task<User?> GetByIdAsync(int id);                   // მომხმარებლის მიღება ID-ის მიხედვით
        Task<User?> GetByPhoneAsync(string phone);          // მომხმარებლის მიღება ტელეფონის ნომრის მიხედვით
        Task<User> AddAsync(User user);                     // ახალი მომხმარებლის დამატება
        Task<User> DeleteUserById(int id);                  // მომხმარებლის წაშლა ID-ის მიხედვით
    }
}
