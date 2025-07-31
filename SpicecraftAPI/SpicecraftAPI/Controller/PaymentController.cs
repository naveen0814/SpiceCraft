using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using Microsoft.AspNetCore.Authorization;
namespace SpicecraftAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null) return NotFound("Payment not found.");
            return Ok(payment);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _paymentService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.PaymentId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePaymentDto dto)
        {
            if (id != dto.PaymentId) return BadRequest("Mismatched ID.");
            var updated = await _paymentService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Payment not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _paymentService.DeleteAsync(id);
            if (!deleted) return NotFound("Payment not found.");
            return NoContent();
        }
    }
}