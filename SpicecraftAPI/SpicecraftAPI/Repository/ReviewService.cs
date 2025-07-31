using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Data;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.MenuItem)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    MenuId = r.MenuId,
                    MenuItemName = r.MenuItem.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToListAsync();
        }

        public async Task<ReviewDto> GetByIdAsync(int id)
        {
            var r = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.MenuItem)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (r == null) return null;

            return new ReviewDto
            {
                ReviewId = r.ReviewId,
                UserId = r.UserId,
                UserName = r.User.Name,
                MenuId = r.MenuId,
                MenuItemName = r.MenuItem.Name,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            };
        }

        public async Task<ReviewDto> CreateAsync(CreateReviewDto dto)
        {
            var r = new Review
            {
                UserId = dto.UserId,
                MenuId = dto.MenuId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(r);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(r.ReviewId);
        }

        public async Task<ReviewDto> UpdateAsync(int id, UpdateReviewDto dto)
        {
            var r = await _context.Reviews.FindAsync(id);
            if (r == null) return null;

            if (dto.UserId != 0) r.UserId = dto.UserId;
            if (dto.MenuId != 0) r.MenuId = dto.MenuId;
            if (dto.Rating != 0) r.Rating = dto.Rating;
            if (!string.IsNullOrWhiteSpace(dto.Comment)) r.Comment = dto.Comment;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var r = await _context.Reviews.FindAsync(id);
            if (r == null) return false;

            _context.Reviews.Remove(r);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ReviewDto>> GetByMenuIdAsync(int menuId)
        {
            return await _context.Reviews
                .Where(r => r.MenuId == menuId)
                .Include(r => r.User)
                .Include(r => r.MenuItem)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    MenuId = r.MenuId,
                    MenuItemName = r.MenuItem.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToListAsync();
        }

        public async Task<IEnumerable<ReviewDto>> GetByUserIdAsync(int userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                .Include(r => r.MenuItem)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    MenuId = r.MenuId,
                    MenuItemName = r.MenuItem.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToListAsync();
        }
    }
}