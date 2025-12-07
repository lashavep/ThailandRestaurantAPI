using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs.BasketDTO;
using RestaurantAPI.DTOs.ProductDTO;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.BasketRepos.Interfaces;
using RestaurantAPI.Services.BasketServices.Interfaces;

namespace RestaurantAPI.Services.BasketServices.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _repository;
        private readonly ApplicationDbContext _dbContext;

        public BasketService(IBasketRepository repository, ApplicationDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        // პროდუქტის დამატება კალათაში
        public async Task<BasketDto> AddToBasketAsync(BasketPostDto dto)
        {
            var existing = await _repository.GetBasketContainingProductAsync(dto.ProductId, dto.UserId);
            if (existing == null)
            {
                var basket = new Basket
                {
                    Quantity = Math.Clamp(dto.Quantity, 1, 99),
                    Price = dto.Price,
                    ProductId = dto.ProductId,
                    UserId = dto.UserId
                };

                await _repository.AddAsync(basket);
            }
            else
            {
                existing.Quantity = Math.Clamp(existing.Quantity + dto.Quantity, 1, 9999);
                existing.Price = dto.Price;
                await _repository.UpdateAsync(existing);
            }

            // დაბრუნება სრული კალათის DTO
            return await BuildBasketDto(dto.UserId);
        }

        // ყველა პროდუქტის მიღება კალათიდან
        public async Task<BasketDto> GetAllAsync(int userId)
        {
            return await BuildBasketDto(userId);
        }

        // კალათის პროდუქტის განახლება
        public async Task<bool> UpdateBasketAsync(UpdateBasketDto dto)
        {
            var basket = await _repository.GetBasketContainingProductAsync(dto.ProductId, dto.UserId);
            if (basket != null)
            {
                basket.Quantity = Math.Clamp(dto.Quantity, 1, 99);
                basket.Price = dto.Price;
                await _repository.UpdateAsync(basket);
                return true;
            }
            return false;
        }

        // პროდუქტის წაშლა კალათიდან
        public async Task<bool> DeleteProductAsync(int productId, int userId)
        {
            var existing = await _repository.GetBasketContainingProductAsync(productId, userId);
            if (existing != null)
            {
                await _repository.DeleteAsync(productId, userId);
                return true;
            }
            return false;
        }

        // კალათის გასუფთავება
        public async Task ClearBasketAsync(int userId)
        {
            var items = await _dbContext.Baskets.Where(b => b.UserId == userId).ToListAsync();
            _dbContext.Baskets.RemoveRange(items);
            await _dbContext.SaveChangesAsync();
        }

        // Helper: ააწყოს BasketDto კონკრეტული UserId-სთვის
        private async Task<BasketDto> BuildBasketDto(int userId)
        {
            var entities = await _repository.GetAllByUserAsync(userId);

            return new BasketDto
            {
                UserId = userId,
                Items = entities.Select(MapToItemDto).ToList()
            };
        }

        // Helper: Basket → BasketItemDto
        private BasketItemDto MapToItemDto(Basket b) => new BasketItemDto
        {
            Id = b.Id,
            Quantity = b.Quantity,
            Price = b.Price,
            ProductId = b.ProductId,
            Product = new ProductDto
            {
                Id = b.Product!.Id,
                Name = b.Product.Name,
                Price = b.Product.Price,
                Image = b.Product.Image,
                Nuts = b.Product.Nuts,
                Vegeterian = b.Product.Vegeterian,
                Spiciness = b.Product.Spiciness,
                CategoryId = b.Product.CategoryId
            }
        };
    }
}
