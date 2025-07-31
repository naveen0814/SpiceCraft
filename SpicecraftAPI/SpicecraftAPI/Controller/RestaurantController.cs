using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;

namespace SpicecraftAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _svc;
        private readonly IOrderService _orderService;

        public RestaurantController(IRestaurantService svc, IOrderService orderService)
        {
            _svc = svc;
            _orderService = orderService;
        }

        // Helper to get the current RestaurantId from JWT
        private int CurrentId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? throw new InvalidOperationException("ID claim missing");
            return int.Parse(raw);
        }

        private bool IsAdmin() => User.IsInRole("Admin");
        private bool IsRestaurant() => User.IsInRole("Restaurant");
        private bool IsUser() => User.IsInRole("User");

        // [1] GET all restaurants (public)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _svc.GetAllRawAsync();
            return Ok(all);
        }

        // [2] GET a single restaurant by PK
        [Authorize(Roles = "Admin,Restaurant,User")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var r = await _svc.GetByIdRawAsync(id);
            if (r == null) return NotFound();

            if (IsAdmin()) return Ok(r);
            if (IsRestaurant() && id == CurrentId()) return Ok(r);
            if (IsUser() && r.UserId == CurrentId()) return Ok(r);

            return Forbid();
        }

        // [3] GET YOUR own restaurant, include LogoUrl
        [HttpGet("me")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> GetMyRestaurant()
        {
            int userId = CurrentId();
            var restaurant = await _svc.GetRestaurantEntityByUserIdAsync(userId);
            if (restaurant == null) return NotFound("Restaurant not found.");

            // Map to DTO
            var dto = new RestaurantDto
            {
                RestaurantId = restaurant.RestaurantId,
                Name = restaurant.Name,
                Email = restaurant.Email,
                ContactNumber = restaurant.ContactNumber,
                Location = restaurant.Location,
                CreatedAt = restaurant.CreatedAt,
                IsApproved = restaurant.IsApproved,
                LogoUrl = restaurant.LogoPath
            };

            return Ok(dto);
        }

        // [4] POST new restaurant application, accepts Logo
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateRestaurantDto dto)
        {
            var created = await _svc.CreateRawAsync(dto, CurrentId());
            return CreatedAtAction(nameof(Get), new { id = created.RestaurantId }, created);
        }

        // [5] PUT update a restaurant
        [Authorize(Roles = "Admin,Restaurant")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateRestaurantDto dto)
        {
            var existing = await _svc.GetByIdRawAsync(id);
            if (existing == null) return NotFound();
            if (!IsAdmin() && !(IsRestaurant() && id == CurrentId())) return Forbid();

            var updated = await _svc.UpdateRawAsync(id, dto);
            return Ok(updated);
        }

        // [6] DELETE a restaurant
        [Authorize(Roles = "Admin,Restaurant")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _svc.GetByIdRawAsync(id);
            if (existing == null) return NotFound();
            if (!IsAdmin() && !(IsRestaurant() && id == CurrentId())) return Forbid();

            await _svc.DeleteAsync(id);
            return NoContent();
        }

        // [6.1] PUT update own restaurant profile (including Logo)
        [HttpPut("me")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> UpdateOwnProfile([FromForm] UpdateRestaurantDto dto)
        {
            var result = await _svc.UpdateOwnAsync(CurrentId(), dto);
            if (result == null) return BadRequest("Update failed");
            return Ok(result);
        }

        // [6.2] View restaurant's orders
        [HttpGet("orders")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> GetOrdersForRestaurant()
        {
            var orders = await _orderService.GetOrdersForRestaurantAsync(CurrentId());
            return Ok(orders);
        }

        // [7] Approve a restaurant (admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("approve/{id:int}")]
        public async Task<IActionResult> Approve(int id)
            => (await _svc.ApproveAsync(id))
                ? Ok("Restaurant approved.")
                : NotFound("Restaurant not found.");
    }
}
