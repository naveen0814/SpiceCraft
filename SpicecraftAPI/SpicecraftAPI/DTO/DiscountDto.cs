using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SpicecraftAPI.DTO
{
    public class DiscountDto
    {
        public int DiscountId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int MenuId { get; set; }
        public string MenuItemName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class CreateDiscountDto
    {
        public int RestaurantId { get; set; }
        public int MenuId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class UpdateDiscountDto
    {
        public int DiscountId { get; set; }
        public int RestaurantId { get; set; }
        public int MenuId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
