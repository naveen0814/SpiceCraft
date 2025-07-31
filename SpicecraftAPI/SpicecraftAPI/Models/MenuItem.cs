using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class MenuItem
    {
        [Key]
        public int MenuId { get; set; }
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
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

        public Restaurant Restaurant { get; set; }
        public MenuCategory Category { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Discount> Discounts { get; set; }
    }
}
