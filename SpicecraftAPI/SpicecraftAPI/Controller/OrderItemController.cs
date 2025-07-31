using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Models;
using SpicecraftAPI.Data;

namespace SpicecraftAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;
        private readonly ApplicationDbContext _context;

        public OrderItemController(IOrderItemService orderItemService, ApplicationDbContext context)
        {
            _orderItemService = orderItemService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _orderItemService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _orderItemService.GetByIdAsync(id);
            if (item == null) return NotFound("Order item not found.");
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _orderItemService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.OrderItemId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOrderItemDto dto)
        {
            if (id != dto.OrderItemId) return BadRequest("Mismatched ID.");
            var updated = await _orderItemService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Order item not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _orderItemService.DeleteAsync(id);
            if (!deleted) return NotFound("Order item not found.");
            return NoContent();
        }
        [HttpPost("place-from-cart")]
        public async Task<IActionResult> PlaceOrderFromCart()
        {
            int userId = GetCurrentUserId();

            // Get user cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.MenuItem)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems.Count == 0)
                return BadRequest("Cart is empty or not found.");

            // Create Order
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                PaymentStatus = "Pending",
                OrderStatus = "Processing",
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.MenuItem.Price)
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create OrderItems
            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    Price = item.MenuItem.Price
                };
                _context.OrderItems.Add(orderItem);
            }

            // Assign free delivery partner
            var freePartner = await _context.DeliveryPartners
                .FirstOrDefaultAsync(dp => !_context.Orders.Any(o => o.DeliveryPartnerId == dp.DeliveryPartnerId && o.OrderStatus != "Delivered"));

            if (freePartner != null)
            {
                order.DeliveryPartnerId = freePartner.DeliveryPartnerId;
                await _context.SaveChangesAsync();
            }

            // Optional: Clear Cart
            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Order placed successfully.",
                OrderId = order.OrderId,
                AssignedDeliveryPartner = freePartner?.Name ?? "Not Assigned"
            });

        }

        private int GetCurrentUserId()
        {
            // This logic extracts the user ID from the claims in your JWT token
            var claim = User.Claims.FirstOrDefault(c =>
                c.Type == System.Security.Claims.ClaimTypes.NameIdentifier ||
                c.Type == "id" ||
                c.Type == "sub");

            if (claim == null)
                throw new InvalidOperationException("User ID claim is missing from the token.");

            if (!int.TryParse(claim.Value, out int userId))
                throw new InvalidOperationException("User ID claim is not a valid integer.");

            return userId;
        }
        [HttpGet("byOrder/{orderId}")]
        public async Task<IActionResult> GetItemsByOrderId(int orderId)
        {
            var items = await _orderItemService.GetItemsByOrderIdAsync(orderId);
            return Ok(items);
        }
    }
}