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

        public async Task<BasketDTO> AddToBasketAsync(BasketPostDto dto)
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

                var saved = await _repository.AddAsync(basket);

                return MapToDto(saved);
            }

            existing.Quantity = Math.Clamp(existing.Quantity + dto.Quantity, 1, 9999);
            existing.Price = dto.Price;
            await _repository.UpdateAsync(existing);

            return MapToDto(existing);
        }

        public async Task<IEnumerable<BasketDTO>> GetAllAsync(int userId)
        {
            var entities = await _repository.GetAllByUserAsync(userId);
            return entities.Select(MapToDto);
        }

        public async Task<bool> UpdateBasketAsync(UpdateBasketDto dto)
        {
            var basket = await _repository.GetBasketContainingProductAsync(dto.ProductId, dto.UserId);
            if (basket != null)
            {
                basket.Quantity = Math.Clamp(dto.Quantity, 1, 99);
                basket.Price = dto.Price;
                await _repository.UpdateAsync(basket);
            }
            return basket != null;
        }

        public async Task<bool> DeleteProductAsync(int productId, int userId)
        {
            var existing = await _repository.GetBasketContainingProductAsync(productId, userId);
            if (existing != null)
            {
                await _repository.DeleteAsync(productId, userId);
            }
            return existing != null;
        }

        private BasketDTO MapToDto(Basket b) => new BasketDTO
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

        public async Task ClearBasketAsync(int userId)
        {
            var items = await _dbContext.Baskets.Where(b => b.UserId == userId).ToListAsync();
            _dbContext.Baskets.RemoveRange(items);
            await _dbContext.SaveChangesAsync();
        }

    }

}
