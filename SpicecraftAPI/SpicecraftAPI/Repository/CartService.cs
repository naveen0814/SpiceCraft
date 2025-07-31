using SpicecraftAPI.Data;
using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartDto>> GetAllAsync()
        {
            return await _context.Carts.Include(c => c.User)
                .Select(c => new CartDto
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    CreatedAt = c.CreatedAt
                }).ToListAsync();
        }

        public async Task<CartDto> GetByIdAsync(int id)
        {
            var c = await _context.Carts.Include(c => c.User).FirstOrDefaultAsync(c => c.CartId == id);
            if (c == null) return null;

            return new CartDto
            {
                CartId = c.CartId,
                UserId = c.UserId,
                UserName = c.User.Name,
                CreatedAt = c.CreatedAt
            };
        }

        public async Task<CartDto> CreateAsync(CreateCartDto dto)
        {
            var exists = await _context.Carts.AnyAsync(c => c.UserId == dto.UserId);
            if (exists)
                throw new InvalidOperationException("User already has a cart.");
            var cart = new Cart
            {
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(cart.CartId);
        }

        public async Task<CartDto> UpdateAsync(int id, UpdateCartDto dto)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return null;

            cart.UserId = dto.UserId ?? cart.UserId;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null) return false;

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CartItemDto>> GetItemsByCartIdAsync(int cartId)
        {
            return await _context.CartItems
                .Where(i => i.CartId == cartId)
                .Include(i => i.MenuItem)
                .Select(i => new CartItemDto
                {
                    CartItemId = i.CartItemId,
                    CartId = i.CartId,
                    MenuId = i.MenuId,
                    MenuItemName = i.MenuItem.Name,
                    Quantity = i.Quantity
                }).ToListAsync();
        }
        public async Task<IEnumerable<CartDto>> GetByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.User)
                .Select(c => new CartDto
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }
    }
}
