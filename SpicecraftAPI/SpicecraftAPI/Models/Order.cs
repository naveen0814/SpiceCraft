using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }      // <-- ADD THIS
        public string PaymentStatus { get; set; }    // <-- ADD THIS
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? DeliveryPartnerId { get; set; }
        [ForeignKey("DeliveryPartnerId")]
        public DeliveryPartner DeliveryPartner { get; set; }
        public User User { get; set; }
        public Restaurant Restaurant { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Payment Payment { get; set; }
    }
}