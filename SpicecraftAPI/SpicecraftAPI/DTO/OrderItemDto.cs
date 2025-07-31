using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SpicecraftAPI.DTO
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuId { get; set; }
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class CreateOrderItemDto
    {
        public int OrderId { get; set; }
        public int MenuId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateOrderItemDto : CreateOrderItemDto
    {
        public int OrderItemId { get; set; }
    }

}
