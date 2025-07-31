using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class Restaurant
    {
        [Key]
        public int RestaurantId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(15)]
        public string ContactNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        [StringLength(300)]
        public string BankDetails { get; set; }

        [StringLength(20)]
        public string GSTIN { get; set; }

        [StringLength(20)]
        public string PANCardNumber { get; set; }

        [StringLength(500)]
        public string FSSAILicensePath { get; set; }

        [StringLength(500)]
        public string GSTDocumentPath { get; set; }

        [StringLength(500)]
        public string LogoPath { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; } = false;

        // Optional: Link to a User entity if using authentication system
        public int? UserId { get; set; } // force EF to see change
        [ForeignKey("UserId")]
        public User User { get; set; }

        // Navigation Properties
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
    }
}
