using SpicecraftAPI.Data;
using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class CartItemService : ICartItemService
    {
        private readonly ApplicationDbContext _context;

        public CartItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItemDto>> GetAllAsync()
        {
            return await _context.CartItems
                .Include(i => i.MenuItem)
                .Include(i => i.Cart)
                .Select(i => new CartItemDto
                {
                    CartItemId = i.CartItemId,
                    CartId = i.CartId,
                    MenuId = i.MenuId,
                    MenuItemName = i.MenuItem.Name,
                    MenuItemPrice = i.MenuItem.Price, // Added price
                    Quantity = i.Quantity,
                    TotalPrice = i.Quantity * i.MenuItem.Price // Added total
                }).ToListAsync();
        }

        public async Task<CartItemDto> GetByIdAsync(int id)
        {
            var item = await _context.CartItems
                .Include(i => i.MenuItem)
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.CartItemId == id);

            if (item == null) return null;

            return new CartItemDto
            {
                CartItemId = item.CartItemId,
                CartId = item.CartId,
                MenuId = item.MenuId,
                MenuItemName = item.MenuItem.Name,
                MenuItemPrice = item.MenuItem.Price,
                Quantity = item.Quantity,
                TotalPrice = item.Quantity * item.MenuItem.Price
            };
        }

        public async Task<CartItemDto> CreateAsync(CreateCartItemDto dto)
        {
            // Verify the cart belongs to the user
            var cart = await _context.Carts.FindAsync(dto.CartId);
            if (cart == null)
                throw new Exception("Cart not found");

            var menuItem = await _context.MenuItems.FindAsync(dto.MenuId);
            if (menuItem == null)
                throw new Exception("Menu item not found");

            CartItem targetItem;

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == dto.CartId && ci.MenuId == dto.MenuId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                targetItem = existingItem;
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = dto.CartId,
                    MenuId = dto.MenuId,
                    Quantity = dto.Quantity
                };
                _context.CartItems.Add(newItem);
                targetItem = newItem;
            }

            await _context.SaveChangesAsync();

            // Now that it’s saved, we can get the full item with navigation props
            return await GetByIdAsync(targetItem.CartItemId);
        }

        public async Task<CartItemDto> UpdateAsync(int id, UpdateCartItemDto dto)
        {
            var item = await _context.CartItems
                .Include(i => i.MenuItem)
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.CartItemId == id);

            if (item == null)
                throw new Exception("Cart item not found");

            if (dto.Quantity.HasValue)
            {
                if (dto.Quantity.Value <= 0)
                {
                    // Remove item if quantity is 0 or less
                    _context.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = dto.Quantity.Value;
                }
            }

            await _context.SaveChangesAsync();

            if (dto.Quantity.HasValue && dto.Quantity.Value <= 0)
                return null; // Item was removed

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null)
                return false;

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CartItemDto>> GetByUserIdAsync(int userId)
        {
            return await _context.CartItems
                .Include(i => i.MenuItem)
                .Include(i => i.Cart)
                .Where(i => i.Cart.UserId == userId)
                .Select(i => new CartItemDto
                {
                    CartItemId = i.CartItemId,
                    CartId = i.CartId,
                    MenuId = i.MenuId,
                    MenuItemName = i.MenuItem.Name,
                    MenuItemPrice = i.MenuItem.Price,
                    Quantity = i.Quantity,
                    TotalPrice = i.Quantity * i.MenuItem.Price
                })
                .ToListAsync();
        }

        public async Task<bool> IsCartOwnedByUserAsync(int cartId, int userId)
        {
            return await _context.Carts
                .AnyAsync(c => c.CartId == cartId && c.UserId == userId);
        }

        public async Task<bool> IsCartItemOwnedByUserAsync(int cartItemId, int userId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .AnyAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == userId);
        }

        public async Task ClearCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Include(ci => ci.Cart)
                .Where(ci => ci.Cart.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}