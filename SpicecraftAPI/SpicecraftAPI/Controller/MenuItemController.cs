// Controllers/MenuItemController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpicecraftAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IRestaurantService _restaurantService;

        public MenuItemController(
            IMenuItemService menuItemService,
            IRestaurantService restaurantService)
        {
            _menuItemService = menuItemService;
            _restaurantService = restaurantService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            return userIdClaim != null
                ? int.Parse(userIdClaim.Value)
                : 0;
        }

        private bool IsAdmin() => User.IsInRole("Admin");
        private bool IsRestaurant() => User.IsInRole("Restaurant");

        // GET: api/MenuItem
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            if (User.Identity?.IsAuthenticated == true && IsRestaurant())
            {
                var userId = GetCurrentUserId();
                var restaurant = await _restaurantService
                    .GetRestaurantEntityByUserIdAsync(userId);
                if (restaurant == null)
                    return Unauthorized("Restaurant account not found for the current user.");

                var items = await _menuItemService
                    .GetByRestaurantIdAsync(restaurant.RestaurantId);
                return Ok(items);
            }

            var allItems = await _menuItemService.GetAllAsync();
            return Ok(allItems);
        }

        // GET: api/MenuItem/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _menuItemService.GetByIdAsync(id);
            if (item == null) return NotFound("Menu item not found.");
            return Ok(item);
        }

        // POST: api/MenuItem
        [HttpPost]
        [Authorize(Roles = "Admin,Restaurant")]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }

            if (IsRestaurant())
            {
                var userId = GetCurrentUserId();
                var restaurant = await _restaurantService
                    .GetRestaurantEntityByUserIdAsync(userId);
                if (restaurant == null || restaurant.RestaurantId != dto.RestaurantId)
                    return Forbid("You can only create items for your own restaurant.");
            }

            var created = await _menuItemService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.MenuId }, created);
        }

        // PUT: api/MenuItem/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Restaurant")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (IsRestaurant())
            {
                var userId = GetCurrentUserId();
                var restaurant = await _restaurantService
                    .GetRestaurantEntityByUserIdAsync(userId);
                var existing = await _menuItemService.GetByIdAsync(id);
                if (restaurant == null || existing.RestaurantId != restaurant.RestaurantId)
                    return Forbid("You can only update your own restaurant's menu items.");
            }

            var updated = await _menuItemService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Menu item not found.");
            return Ok(updated);
        }

        // DELETE: api/MenuItem/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Restaurant")]
        public async Task<IActionResult> Delete(int id)
        {
            if (IsRestaurant())
            {
                var userId = GetCurrentUserId();
                var restaurant = await _restaurantService
                    .GetRestaurantEntityByUserIdAsync(userId);
                var existing = await _menuItemService.GetByIdAsync(id);
                if (restaurant == null || existing.RestaurantId != restaurant.RestaurantId)
                    return Forbid("You can only delete your own restaurant's menu items.");
            }

            var deleted = await _menuItemService.DeleteAsync(id);
            if (!deleted) return NotFound("Menu item not found.");
            return NoContent();
        }

        // GET: api/MenuItem/byCategory/{categoryId}
        [HttpGet("byCategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var items = await _menuItemService.GetByCategoryIdAsync(categoryId);
            return Ok(items);
        }

        // GET: api/MenuItem/byRestaurant/{restaurantId}
        [HttpGet("byRestaurant/{restaurantId}")]
        [Authorize(Roles = "Admin,Restaurant")]
        public async Task<IActionResult> GetByRestaurant(int restaurantId)
        {
            if (IsRestaurant())
            {
                var userId = GetCurrentUserId();
                var restaurant = await _restaurantService
                    .GetRestaurantEntityByUserIdAsync(userId);
                if (restaurant == null || restaurant.RestaurantId != restaurantId)
                    return Forbid("You can only view your own restaurant's items.");
            }

            var items = await _menuItemService.GetByRestaurantIdAsync(restaurantId);
            return Ok(items);
        }

        // GET: api/MenuItem/public
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicMenuItems()
        {
            var items = await _menuItemService.GetAllAsync();
            var available = items.Where(i => i.IsAvailable);
            return Ok(available);
        }

        // GET: api/MenuItem/my-restaurant
        [HttpGet("my-restaurant")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> GetMenuItemsForCurrentRestaurant()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized("User ID not found in token.");

                var restaurant = await _restaurantService
                    .GetRestaurantEntityByUserIdAsync(userId);
                if (restaurant == null)
                    return Unauthorized("Restaurant account not found for the current user.");

                var items = await _menuItemService
                    .GetByRestaurantIdAsync(restaurant.RestaurantId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in /my-restaurant: " + ex.Message);
                return StatusCode(500, "An error occurred while fetching menu items.");
            }
        }

        // GET: api/MenuItem/search?q=term[&restaurantId=5]
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(
            [FromQuery] string q,
            [FromQuery] int? restaurantId)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Query parameter 'q' cannot be empty.");

            IEnumerable<MenuItemDto> items;
            if (restaurantId.HasValue)
            {
                items = await _menuItemService
                    .SearchByRestaurantAsync(restaurantId.Value, q);
            }
            else
            {
                items = await _menuItemService.SearchAsync(q);
            }

            return Ok(items);
        }
    }
}
