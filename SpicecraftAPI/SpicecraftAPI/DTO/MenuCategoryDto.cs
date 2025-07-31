using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SpicecraftAPI.DTO
{
    public class MenuCategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }
    public class CreateMenuCategoryDto
    {
        public string Name { get; set; }
    }

    public class UpdateMenuCategoryDto
    {
        public string Name { get; set; }
    }
}
