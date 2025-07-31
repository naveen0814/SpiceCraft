using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using Microsoft.AspNetCore.Authorization;

namespace SpicecraftAPI.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyCart()
        {
            int userId = GetCurrentUserId();
            var carts = await _cartService.GetByUserIdAsync(userId);
            return Ok(carts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cart = await _cartService.GetByIdAsync(id);
            int userId = GetCurrentUserId();
            if (cart == null) return NotFound("Cart not found.");
            if (cart.UserId != userId) return Forbid("You can only access your own cart.");
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            int userId = GetCurrentUserId();

            var existing = await _cartService.GetByUserIdAsync(userId);
            if (existing.Any())
                return BadRequest("User already has a cart.");

            var dto = new CreateCartDto { UserId = userId };
            var result = await _cartService.CreateAsync(dto);

            return CreatedAtAction(nameof(Get), new { id = result.CartId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCartDto dto)
        {
            int userId = GetCurrentUserId();
            if (id != dto.CartId) return BadRequest("Mismatched Cart ID.");

            var cart = await _cartService.GetByIdAsync(id);
            if (cart == null) return NotFound("Cart not found.");
            if (cart.UserId != userId) return Forbid("You can only update your own cart.");

            var result = await _cartService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetCurrentUserId();
            var cart = await _cartService.GetByIdAsync(id);
            if (cart == null) return NotFound("Cart not found.");
            if (cart.UserId != userId) return Forbid("You can only delete your own cart.");

            var deleted = await _cartService.DeleteAsync(id);
            if (!deleted) return NotFound("Cart not found.");
            return NoContent();
        }

        [HttpGet("{cartId}/items")]
        public async Task<IActionResult> GetCartItems(int cartId)
        {
            int userId = GetCurrentUserId();
            var cart = await _cartService.GetByIdAsync(cartId);
            if (cart == null) return NotFound("Cart not found.");
            if (cart.UserId != userId) return Forbid("You can only view items in your own cart.");

            var items = await _cartService.GetItemsByCartIdAsync(cartId);
            return Ok(items);
        }
    }
}
