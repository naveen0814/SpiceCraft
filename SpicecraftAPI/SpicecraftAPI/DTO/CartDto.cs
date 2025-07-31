using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SpicecraftAPI.DTO
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CreateCartDto
    {
        public int UserId { get; set; }
    }

    public class UpdateCartDto
    {
        public int CartId { get; set; }
        public int? UserId { get; set; } // nullable
    }
}
