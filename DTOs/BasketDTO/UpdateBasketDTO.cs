using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.BasketDTO
{
    public class UpdateBasketDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public int UserId { get; set; }
    }

}
