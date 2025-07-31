using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class MenuCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; }
    }
}