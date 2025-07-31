using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;

namespace SpicecraftAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryPartnerController : ControllerBase
    {
        private readonly IDeliveryPartnerService _service;
        public DeliveryPartnerController(IDeliveryPartnerService service)
            => _service = service;

        // USER applies to be a delivery partner
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Apply([FromForm] CreateDeliveryPartnerDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var created = await _service.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.DeliveryPartnerId }, created);
        }

        // USER/ADMIN: See my own application
        [Authorize(Roles = "User,Admin")]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyApplication()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var app = await _service.GetByUserIdAsync(userId);
            if (app == null) return NotFound("No application found.");
            return Ok(app);
        }

        // ADMIN: Approve application
        [Authorize(Roles = "Admin")]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
            => (await _service.ApproveAsync(id))
                ? Ok("Approved")
                : NotFound("Delivery partner not found.");

        // DELIVERY PARTNER: View assigned orders
        [Authorize(Roles = "DeliveryPartner")]
        [HttpGet("assigned-orders")]
        public async Task<IActionResult> AssignedOrders()
        {
            var rawId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(rawId, out int partnerId))
                return Unauthorized();
            var list = await _service.GetAssignedOrdersAsync(partnerId);
            return Ok(list);
        }

        // DELIVERY PARTNER: Accept an order
        [Authorize(Roles = "DeliveryPartner")]
        [HttpPut("accept-order/{orderId}")]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            var partnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return (await _service.AcceptOrderAsync(partnerId, orderId))
                ? Ok("Order accepted.")
                : NotFound("Order not found or not assigned to you.");
        }

        // DELIVERY PARTNER: See order history
        [Authorize(Roles = "DeliveryPartner")]
        [HttpGet("history")]
        public async Task<IActionResult> History()
        {
            var partnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _service.GetOrderHistoryAsync(partnerId));
        }

        // DELIVERY PARTNER: Earnings summary
        [Authorize(Roles = "DeliveryPartner")]
        [HttpGet("earnings")]
        public async Task<IActionResult> Earnings()
        {
            var partnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _service.GetEarningsAsync(partnerId));
        }

        // DELIVERY PARTNER: Update order status (PATCH)
        [Authorize(Roles = "DeliveryPartner")]
        [HttpPatch("order/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            var partnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!await _service.UpdateAssignedOrderStatusAsync(partnerId, orderId, dto.OrderStatus))
                return BadRequest("Order not found or not assigned to you.");
            return Ok("Order status updated.");
        }

        // ADMIN: List all partners
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var partners = await _service.GetAllAsync();
            return Ok(partners);
        }

        // ADMIN/PARTNER: Get by id
        [Authorize(Roles = "Admin,DeliveryPartner,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var partner = await _service.GetByIdAsync(id);
            if (partner == null) return NotFound();
            return Ok(partner);
        }

        // ADMIN/DELIVERY PARTNER: Update a partner
        [Authorize(Roles = "Admin,DeliveryPartner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateDeliveryPartnerDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Delivery partner not found.");
            return Ok(updated);
        }

        // ADMIN: Delete a partner
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound("Delivery partner not found.");
            return NoContent();
        }
    }
}
