
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantAPI.Data;
using RestaurantAPI.Hubs.NotificationHub;
using RestaurantAPI.Repositories.AdminRepos.Implementations.RestaurantAPI.Repositories.AdminRepos.Implementations;
using RestaurantAPI.Repositories.AdminRepos.Interfaces;
using RestaurantAPI.Repositories.BasketRepos.Implementations;
using RestaurantAPI.Repositories.BasketRepos.Interfaces;
using RestaurantAPI.Repositories.CategoryRepos.Implementations;
using RestaurantAPI.Repositories.CategoryRepos.Interfaces;
using RestaurantAPI.Repositories.OrderRepos.Implementations;
using RestaurantAPI.Repositories.OrderRepos.Interfaces;
using RestaurantAPI.Repositories.ProductRepos.Implementatios;
using RestaurantAPI.Repositories.ProductRepos.Interfaces;
using RestaurantAPI.Repositories.UserRepos.Implementations;
using RestaurantAPI.Repositories.UserRepos.Interfaces;
using RestaurantAPI.Services.AdminServices.Implementations.RestaurantAPI.Services.AdminServices.Implementations;
using RestaurantAPI.Services.AdminServices.Interfaces;
using RestaurantAPI.Services.AuthServices.Implementations;
using RestaurantAPI.Services.AuthServices.Interfaces;
using RestaurantAPI.Services.BasketServices.Implementations;
using RestaurantAPI.Services.BasketServices.Interfaces;
using RestaurantAPI.Services.CategoryServices.Implementations;
using RestaurantAPI.Services.CategoryServices.Interfaces;
using RestaurantAPI.Services.EmailService.Implementations;
using RestaurantAPI.Services.EmailService.Interfaces;
using RestaurantAPI.Services.OrderServices.Interfaces;
using RestaurantAPI.Services.ProductServices.Implementations;
using RestaurantAPI.Services.ProductServices.Interfaces;
using RestaurantAPI.Services.UserServices.Implementations;
using RestaurantAPI.Services.UserServices.Interfaces;


namespace RestaurantAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            

            builder.Services.AddDbContext<Data.ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IBasketRepository, BasketRepository>();
            builder.Services.AddScoped<IBasketService, BasketService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IEmailService, EmailService>();



            builder.Services.AddSignalR();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "RestaurantAPI", Version = "v1" });

                
                c.AddSecurityDefinition("Bearer", new() 
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token"
                });

                c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); 
                });
            });

            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "change_this_secret_in_prod");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
            });




            var app = builder.Build();

            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAngularApp");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<NotificationHub>("/notificationHub");

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                DataSeeder.Seed(db);
            }

            app.Run();
        }
    }
}
