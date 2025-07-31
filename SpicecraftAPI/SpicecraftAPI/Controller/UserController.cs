using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SpicecraftAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        private bool IsAdmin() => User.IsInRole("Admin");

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            int currentUserId = GetCurrentUserId();
            if (!IsAdmin() && id != currentUserId)
                return Forbid("You can only access your own profile.");

            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.UserId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserDto dto)
        {
            int currentUserId = GetCurrentUserId();
            if (!IsAdmin() && id != currentUserId)
                return Forbid("You can only update your own profile.");

            var updated = await _userService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("User not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int currentUserId = GetCurrentUserId();
            if (!IsAdmin() && id != currentUserId)
                return Forbid("You can only delete your own account.");

            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound("User not found.");
            return NoContent();
        }

        [HttpGet("email/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }
    }
}
