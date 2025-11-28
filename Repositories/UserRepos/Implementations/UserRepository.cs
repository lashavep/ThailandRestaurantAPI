using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.UserRepos.Interfaces;

namespace RestaurantAPI.Repositories.UserRepos.Implementations
{
        public class UserRepository : IUserRepository
        {
            private readonly ApplicationDbContext _db;
            public UserRepository(ApplicationDbContext db) => _db = db;


        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
                => await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            public async Task<User?> GetByIdAsync(int id)
                => await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<bool> UpdateAsync(User user)
        {
            _db.Users.Update(user);
            return await _db.SaveChangesAsync() > 0;
        }


        public async Task<User?> GetByPhoneAsync(string phone)
            {
                return await _db.Users.FirstOrDefaultAsync(u => u.Phone == phone);
            }


            public async Task<User> AddAsync(User user)
                {
                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();
                    return user;
                }

            public async Task<User> DeleteUserById(int id)
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    _db.Users.Remove(user);
                    await _db.SaveChangesAsync();
                    return user;
                }
                throw new InvalidOperationException($"User with id {id} not found.");
        }
    }
}
