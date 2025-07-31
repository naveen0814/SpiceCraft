using SpicecraftAPI.Data;
using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class DiscountService : IDiscountService
    {
        private readonly ApplicationDbContext _context;

        public DiscountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiscountDto>> GetAllAsync()
        {
            var discounts = await _context.Discounts
                .Include(d => d.Restaurant)
                .Include(d => d.MenuItem)
                .ToListAsync();

            return discounts.Select(d => new DiscountDto
            {
                DiscountId = d.DiscountId,
                RestaurantId = d.RestaurantId,
                RestaurantName = d.Restaurant?.Name,
                MenuId = d.MenuId,
                MenuItemName = d.MenuItem?.Name,
                DiscountPercentage = d.DiscountPercentage,
                StartDate = d.StartDate,
                EndDate = d.EndDate
            });
        }

        public async Task<DiscountDto> GetByIdAsync(int id)
        {
            var d = await _context.Discounts
                .Include(d => d.Restaurant)
                .Include(d => d.MenuItem)
                .FirstOrDefaultAsync(d => d.DiscountId == id);

            if (d == null) return null;

            return new DiscountDto
            {
                DiscountId = d.DiscountId,
                RestaurantId = d.RestaurantId,
                RestaurantName = d.Restaurant?.Name,
                MenuId = d.MenuId,
                MenuItemName = d.MenuItem?.Name,
                DiscountPercentage = d.DiscountPercentage,
                StartDate = d.StartDate,
                EndDate = d.EndDate
            };
        }

        public async Task<DiscountDto> CreateAsync(CreateDiscountDto dto)
        {
            var discount = new Discount
            {
                RestaurantId = dto.RestaurantId,
                MenuId = dto.MenuId,
                DiscountPercentage = dto.DiscountPercentage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();

            // Re-fetch to include navigation properties
            return await GetByIdAsync(discount.DiscountId);
        }

        public async Task<DiscountDto> UpdateAsync(int id, UpdateDiscountDto dto)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return null;

            discount.RestaurantId = dto.RestaurantId;
            discount.MenuId = dto.MenuId;
            discount.DiscountPercentage = dto.DiscountPercentage;
            discount.StartDate = dto.StartDate;
            discount.EndDate = dto.EndDate;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return false;

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
