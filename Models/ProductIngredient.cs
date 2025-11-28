using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class ProductIngredient
    {
        
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string Ingredients { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product? Product { get; set; } = null;
    }

}
