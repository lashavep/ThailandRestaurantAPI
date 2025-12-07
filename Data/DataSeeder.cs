using Microsoft.AspNetCore.Identity;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data
{
    public static class DataSeeder                                                  // მონაცემთა საწყისი შევსება
    {
        public static void Seed(ApplicationDbContext context)                       // საწყისი ადმინისტრატორის შექმნა
        {
            if (!context.Users.Any(u => u.Email == "foodlab.rs@gmail.com"))         // თუ ადმინისტრატორი არ არსებობს
            {
                var passwordHasher = new PasswordHasher<User>();                    // პაროლის ჰეშერის შექმნა

                var admin = new User                                                // ახალი ადმინისტრატორის შექმნა
                {
                    FirstName = "Admin",
                    Phone = "599905203",
                    Email = "foodlab.rs@gmail.com",
                    Role = "Admin"
                };                                                                  
                    
                admin.PasswordHash = passwordHasher.HashPassword(admin, "admin123"); // პაროლის ჰეშირება

                context.Users.Add(admin);                                            // ადმინისტრატორის დამატება მონაცემთა ბაზაში
            }

            context.SaveChanges();
        }
    }
}
