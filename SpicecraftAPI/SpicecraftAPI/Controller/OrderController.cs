using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using Microsoft.AspNetCore.Authorization;
namespace SpicecraftAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound("Order not found.");
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _orderService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.OrderId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOrderDto dto)
        {
            if (id != dto.OrderId) return BadRequest("Mismatched ID.");
            var updated = await _orderService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Order not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _orderService.DeleteAsync(id);
            if (!deleted) return NotFound("Order not found.");
            return NoContent();
        }
        [Authorize(Roles = "User")]
        [HttpPost("place-from-cart")]
        public async Task<IActionResult> PlaceOrderFromCart()
        {
            // Get current user ID (assuming JWT auth)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            if (userIdClaim == null) return Unauthorized("No user context.");
            int userId = int.Parse(userIdClaim.Value);

            // Place order from cart
            try
            {
                var result = await _orderService.PlaceOrderFromCartAsync(userId);
                return Ok(result); // result = OrderResultDto with order & delivery partner info
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace

                });
                }
        }
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound("Order not found.");

            // You may want to authorize that only delivery partners (or admins) can update to Delivered, etc.

            // Now call a service method to update status
            var updated = await _orderService.UpdateOrderStatusAsync(id, dto.OrderStatus);
            if (!updated)
                return BadRequest("Could not update order status.");

            return Ok("Order status updated.");
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("user")]
        public async Task<IActionResult> GetOrdersForCurrentUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            if (userIdClaim == null) return Unauthorized("User context not found.");

            int userId = int.Parse(userIdClaim.Value);

            var orders = await _orderService.GetByUserIdAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound("No orders found for this user.");

            return Ok(orders);
        }
        [Authorize(Roles = "Restaurant")]
        [HttpGet("restaurant")]
        public async Task<IActionResult> GetOrdersForCurrentRestaurant()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            if (userIdClaim == null)
                return Unauthorized("User context not found.");

            int userId = int.Parse(userIdClaim.Value);

            var orders = await _orderService.GetOrdersForRestaurantAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound("No orders found for this restaurant.");

            return Ok(orders);
        }


    }
}   