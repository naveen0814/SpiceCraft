using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;



namespace SpicecraftAPI.DTO
{
    public class MenuItemDto
    {
        public int MenuId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string AvailableTime { get; set; }
        public bool IsAvailable { get; set; }
        public string DietType { get; set; }
        public string TasteInfo { get; set; }
        public int Calories { get; set; }
        public int Fats { get; set; }
        public int Proteins { get; set; }
        public int Carbohydrates { get; set; }
        public string ImageUrl { get; set; }
    }
    public class CreateMenuItemDto
    {
        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        public string AvailableTime { get; set; } = "";

        public bool IsAvailable { get; set; } = true;

        public string DietType { get; set; } = "";
        public string TasteInfo { get; set; } = "";

        [Range(0, 10000)]
        public int Calories { get; set; }

        [Range(0, 1000)]
        public int Fats { get; set; }

        [Range(0, 1000)]
        public int Proteins { get; set; }

        [Range(0, 1000)]
        public int Carbohydrates { get; set; }

        public string ImageUrl { get; set; } = "";
    }
}

public class UpdateMenuItemDto
    {
        public int? RestaurantId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public string? AvailableTime { get; set; }
        public bool? IsAvailable { get; set; }
        public string? DietType { get; set; }
        public string? TasteInfo { get; set; }
        public int? Calories { get; set; }
        public int? Fats { get; set; }
        public int? Proteins { get; set; }
        public int? Carbohydrates { get; set; }
        public string? ImageUrl { get; set; }
    }


