using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SpicecraftAPI.DTO
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }       // <-- Add this
        public string PaymentStatus { get; set; }     // <-- Add this
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? DeliveryPartnerId { get; set; }
        public string DeliveryPartnerName { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }

    }
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }       // <-- Add this (optional, or set default in code)
        public string PaymentStatus { get; set; }     // <-- Add this (optional, or set default in code)
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
    }

    public class UpdateOrderDto : CreateOrderDto
    {
        public int OrderId { get; set; }
    }
    public class UpdateOrderStatusDto
    {
        public string OrderStatus { get; set; }
    }

}
