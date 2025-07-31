using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SpicecraftAPI.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }
    }

    public class UpdateUserDto
    {
        public int UserId { get; set; } // optional to use, not required if path is used
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public UserRole? Role { get; set; } // enum is nullable to support partial update
    }
}
