using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Data;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class OrderItemService : IOrderItemService
    {
        private readonly ApplicationDbContext _context;

        public OrderItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllAsync()
        {
            return await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    MenuId = oi.MenuId,
                    MenuItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToListAsync();
        }

        public async Task<OrderItemDto> GetByIdAsync(int id)
        {
            var oi = await _context.OrderItems.Include(oi => oi.MenuItem).FirstOrDefaultAsync(oi => oi.OrderItemId == id);
            if (oi == null) return null;

            return new OrderItemDto
            {
                OrderItemId = oi.OrderItemId,
                OrderId = oi.OrderId,
                MenuId = oi.MenuId,
                MenuItemName = oi.MenuItem.Name,
                Quantity = oi.Quantity,
                Price = oi.Price
            };
        }

        public async Task<OrderItemDto> CreateAsync(CreateOrderItemDto dto)
        {
            var oi = new OrderItem
            {
                OrderId = dto.OrderId,
                MenuId = dto.MenuId,
                Quantity = dto.Quantity,
                Price = dto.Price
            };

            _context.OrderItems.Add(oi);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(oi.OrderItemId);
        }

        public async Task<OrderItemDto> UpdateAsync(int id, UpdateOrderItemDto dto)
        {
            var oi = await _context.OrderItems.FindAsync(id);
            if (oi == null) return null;

            if (dto.OrderId != 0) oi.OrderId = dto.OrderId;
            if (dto.MenuId != 0) oi.MenuId = dto.MenuId;
            if (dto.Quantity != 0) oi.Quantity = dto.Quantity;
            if (dto.Price != default) oi.Price = dto.Price;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var oi = await _context.OrderItems.FindAsync(id);
            if (oi == null) return false;

            _context.OrderItems.Remove(oi);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<OrderItemDto>> GetItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.MenuItem)
                .Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    MenuId = oi.MenuId,
                    MenuItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToListAsync();
        }
    }
}