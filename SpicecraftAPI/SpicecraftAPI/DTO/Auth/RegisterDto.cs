using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Gender { get; set; }

    [Required]
    [EnumDataType(typeof(UserRole))] // ✅ helps model binding + Swagger validation
    public UserRole Role { get; set; }
}