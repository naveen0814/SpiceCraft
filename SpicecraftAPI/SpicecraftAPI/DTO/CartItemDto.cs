using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SpicecraftAPI.DTO
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int MenuId { get; set; }
        public string MenuItemName { get; set; }
        public decimal MenuItemPrice { get; set; }   // <-- Add this
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }      // <-- Add this
    }

    public class CreateCartItemDto
    {
        [Required]
        public int CartId { get; set; }

        [Required]
        public int MenuId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

    public class UpdateCartItemDto
    {
        // Removed CartItemId from body because it’s already passed in the route
        public int? CartId { get; set; }
        public int? MenuId { get; set; }
        public int? Quantity { get; set; }
    }
}
