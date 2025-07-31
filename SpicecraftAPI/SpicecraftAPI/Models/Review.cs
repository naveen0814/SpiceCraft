using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}