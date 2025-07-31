using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }

        [Required]
        public UserRole Role { get; set; }  // ✅ Enum type

        public DateTime CreatedAt { get; set; }

        public ICollection<Cart> Carts { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
