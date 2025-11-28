using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.CategoryDTO
{
    public class GetCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
    }

}
