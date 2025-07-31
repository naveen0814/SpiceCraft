using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using Microsoft.AspNetCore.Authorization;
namespace SpicecraftAPI.Controllers
{
    [Authorize(Roles = "Admin,Restaurant")]
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var discounts = await _discountService.GetAllAsync();
            return Ok(discounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var discount = await _discountService.GetByIdAsync(id);
            if (discount == null) return NotFound("Discount not found.");
            return Ok(discount);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDiscountDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _discountService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.DiscountId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDiscountDto dto)
        {
            if (id != dto.DiscountId) return BadRequest("Mismatched ID.");

            var updated = await _discountService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Discount not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _discountService.DeleteAsync(id);
            if (!deleted) return NotFound("Discount not found.");
            return NoContent();
        }
    }
}