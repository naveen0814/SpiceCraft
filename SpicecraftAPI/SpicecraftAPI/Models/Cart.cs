using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}