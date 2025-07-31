using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int MenuId { get; set; }
        public int Quantity { get; set; }

        public Cart Cart { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}