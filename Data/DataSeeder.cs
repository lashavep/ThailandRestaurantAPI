using Microsoft.AspNetCore.Identity;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data
{
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any(u => u.Email == "foodlab.rs@gmail.com"))
            {
                var passwordHasher = new PasswordHasher<User>();

                var admin = new User
                {
                    FirstName = "Admin",
                    Phone = "599905203",
                    Email = "foodlab.rs@gmail.com",
                    Role = "Admin"
                };

                admin.PasswordHash = passwordHasher.HashPassword(admin, "admin123");

                context.Users.Add(admin);
            }

            context.SaveChanges();
        }
    }
}
