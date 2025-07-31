using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SpicecraftAPI.DTO
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int MenuId { get; set; }
        public string MenuItemName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CreateReviewDto
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class UpdateReviewDto : CreateReviewDto
    {
        public int ReviewId { get; set; }
    }

}
