using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Data;
using SpicecraftAPI.DTO.Auth;
using SpicecraftAPI.Models;
using SpicecraftAPI.Services;

namespace SpicecraftAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already registered.");

            // ✅ Force the role to "User" regardless of input
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Gender = dto.Gender,
                Role = UserRole.User, // 👈 Enforce only "User"
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully as 'User'.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var token = _tokenService.GenerateToken(user.UserId, user.Role.ToString(), user.Email);
            return Ok(new { token });
        }
        [HttpPost("login/restaurant")]
        public async Task<IActionResult> LoginRestaurant(RestaurantLoginDto dto)
        {
            var r = await _context.Restaurants.FirstOrDefaultAsync(r => r.Email == dto.Email && r.IsApproved);
            if (r == null || !BCrypt.Net.BCrypt.Verify(dto.Password, r.PasswordHash))
                return Unauthorized("Invalid restaurant credentials or not approved.");

            var token = _tokenService.GenerateToken(r.UserId.Value, "Restaurant", r.Email);
            return Ok(new { token, role = "Restaurant", id = r.RestaurantId });
        }

        [HttpPost("login/delivery")]
        public async Task<IActionResult> LoginDeliveryPartner(DeliveryPartnerLoginDto dto)
        {
            var dp = await _context.DeliveryPartners.FirstOrDefaultAsync(d => d.PhoneNumber == dto.PhoneNumber && d.IsApproved);
            if (dp == null || !BCrypt.Net.BCrypt.Verify(dto.Password, dp.PasswordHash))
                return Unauthorized("Invalid delivery partner credentials or not approved.");

            var token = _tokenService.GenerateToken(dp.DeliveryPartnerId, "DeliveryPartner", dp.PhoneNumber);
            return Ok(new { token, role = "DeliveryPartner", id = dp.DeliveryPartnerId });
        }
    }
}