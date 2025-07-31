using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }
        public int RestaurantId { get; set; }
        public int MenuId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Restaurant Restaurant { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}
