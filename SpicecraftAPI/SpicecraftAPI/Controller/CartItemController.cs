using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using Microsoft.AspNetCore.Authorization;

namespace SpicecraftAPI.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int userId = GetCurrentUserId();
            var items = await _cartItemService.GetByUserIdAsync(userId);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            int userId = GetCurrentUserId();
            var isValid = await _cartItemService.IsCartItemOwnedByUserAsync(id, userId);
            if (!isValid) return Forbid("You cannot access items from another user’s cart.");

            var item = await _cartItemService.GetByIdAsync(id);
            if (item == null) return NotFound("Cart item not found.");
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCartItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int userId = GetCurrentUserId();
            var isValid = await _cartItemService.IsCartOwnedByUserAsync(dto.CartId, userId);
            if (!isValid) return Forbid("You can only add items to your own cart.");

            var created = await _cartItemService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.CartItemId }, created);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCartItemDto dto)
        {
          

            int userId = GetCurrentUserId();
            var isValid = await _cartItemService.IsCartItemOwnedByUserAsync(id, userId);
            if (!isValid) return Forbid("You cannot update items from another user’s cart.");

            var updated = await _cartItemService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Cart item not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetCurrentUserId();
            var isValid = await _cartItemService.IsCartItemOwnedByUserAsync(id, userId);
            if (!isValid) return Forbid("You cannot delete items from another user’s cart.");

            var deleted = await _cartItemService.DeleteAsync(id);
            if (!deleted) return NotFound("Cart item not found.");
            return NoContent();
        }
    }
}