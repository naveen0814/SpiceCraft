using SpicecraftAPI.Data;
using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly ApplicationDbContext _context;

        public MenuCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuCategoryDto>> GetAllAsync()
        {
            return await _context.MenuCategories
                .Select(c => new MenuCategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                }).ToListAsync();
        }

        public async Task<MenuCategoryDto> GetByIdAsync(int id)
        {
            var c = await _context.MenuCategories.FindAsync(id);
            if (c == null) return null;

            return new MenuCategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name
            };
        }

        public async Task<MenuCategoryDto> CreateAsync(CreateMenuCategoryDto dto)
        {
            var category = new MenuCategory { Name = dto.Name };
            _context.MenuCategories.Add(category);
            await _context.SaveChangesAsync();

            return new MenuCategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }

        public async Task<MenuCategoryDto> UpdateAsync(int id, UpdateMenuCategoryDto dto)
        {
            var category = await _context.MenuCategories.FindAsync(id);
            if (category == null) return null;

            category.Name = dto.Name;
            await _context.SaveChangesAsync();

            return new MenuCategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.MenuCategories.FindAsync(id);
            if (category == null) return false;

            _context.MenuCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
