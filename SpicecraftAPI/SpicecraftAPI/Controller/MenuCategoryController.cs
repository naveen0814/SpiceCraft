using Microsoft.AspNetCore.Mvc;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using Microsoft.AspNetCore.Authorization;
namespace SpicecraftAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuCategoryController : ControllerBase
    {
        private readonly IMenuCategoryService _menuCategoryService;

        public MenuCategoryController(IMenuCategoryService menuCategoryService)
        {
            _menuCategoryService = menuCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _menuCategoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _menuCategoryService.GetByIdAsync(id);
            if (category == null) return NotFound("Menu category not found.");
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMenuCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _menuCategoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.CategoryId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateMenuCategoryDto dto)
        {
            var updated = await _menuCategoryService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Category not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _menuCategoryService.DeleteAsync(id);
            if (!deleted) return NotFound("Category not found.");
            return NoContent();
        }
    }
}