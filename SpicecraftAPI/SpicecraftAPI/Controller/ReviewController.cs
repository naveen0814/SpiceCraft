using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using Microsoft.AspNetCore.Authorization;
namespace SpicecraftAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _reviewService.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null) return NotFound("Review not found.");
            return Ok(review);
        }

        [HttpPost]
        [Authorize(Roles = "User,Restaurant")]
        public async Task<IActionResult> Create(CreateReviewDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _reviewService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.ReviewId }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User,Restaurant")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto)
        {
            var updated = await _reviewService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Review not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Restaurant")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _reviewService.DeleteAsync(id);
            if (!deleted) return NotFound("Review not found.");
            return NoContent();
        }

        [HttpGet("byMenu/{menuId}")]
        public async Task<IActionResult> GetByMenu(int menuId)
        {
            var reviews = await _reviewService.GetByMenuIdAsync(menuId);
            return Ok(reviews);
        }

        [HttpGet("byUser/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var reviews = await _reviewService.GetByUserIdAsync(userId);
            return Ok(reviews);
        }
    }
}