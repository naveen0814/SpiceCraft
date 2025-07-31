using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Data;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    UserName = o.User.Name,
                    RestaurantId = o.RestaurantId,
                    RestaurantName = o.Restaurant.Name,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    ShippingAddress = o.ShippingAddress,
                    CreatedAt = o.CreatedAt,
                    DeliveryPartnerId = o.DeliveryPartnerId
                }).ToListAsync();
        }

        public async Task<OrderDto> GetByIdAsync(int id)
        {
            var o = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (o == null) return null;

            return new OrderDto
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                UserName = o.User.Name,
                RestaurantId = o.RestaurantId,
                RestaurantName = o.Restaurant.Name,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus,
                PaymentStatus = o.PaymentStatus,
                PaymentMethod = o.PaymentMethod,
                ShippingAddress = o.ShippingAddress,
                CreatedAt = o.CreatedAt,
                DeliveryPartnerId = o.DeliveryPartnerId
            };
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                RestaurantId = dto.RestaurantId,
                TotalAmount = dto.TotalAmount,
                OrderStatus = "Pending",
                PaymentStatus = "Pending",
                PaymentMethod = dto.PaymentMethod,
                ShippingAddress = dto.ShippingAddress,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(order.OrderId);
        }

        public async Task<OrderDto> UpdateAsync(int id, UpdateOrderDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return null;

            order.OrderStatus = dto.OrderStatus;
            order.PaymentMethod = dto.PaymentMethod;
            order.PaymentStatus = dto.PaymentStatus;
            order.ShippingAddress = dto.ShippingAddress;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OrderResultDto> PlaceOrderFromCartAsync(int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get user cart
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.MenuItem)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || cart.CartItems.Count == 0)
                    throw new Exception("Cart is empty or not found.");

                var totalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.MenuItem.Price);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                string shippingAddress = user?.Address ?? "Unknown Address";

                var order = new Order
                {
                    UserId = userId,
                    RestaurantId = cart.CartItems.First().MenuItem.RestaurantId,
                    TotalAmount = totalAmount,
                    OrderStatus = "Payment Pending",
                    PaymentStatus = "Pending",
                    PaymentMethod = "Online",
                    ShippingAddress = shippingAddress,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var ci in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        MenuId = ci.MenuId,
                        Quantity = ci.Quantity,
                        Price = ci.MenuItem.Price
                    };
                    _context.OrderItems.Add(orderItem);
                }
                await _context.SaveChangesAsync();

                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new OrderResultDto
                {
                    Message = "Order placed successfully. Payment pending.",
                    OrderId = order.OrderId,
                    AssignedDeliveryPartner = "Not assigned yet"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;
            order.OrderStatus = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersForRestaurantAsync(int userId)
        {
            // First, map the restaurant's UserId to their RestaurantId
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (restaurant == null)
                return Enumerable.Empty<OrderDto>();

            int restaurantId = restaurant.RestaurantId;

            return await _context.Orders
                .Where(o => o.RestaurantId == restaurantId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        MenuItemName = oi.MenuItem.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<OrderDto>> GetByUserIdAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Restaurant)
                .Include(o => o.User)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                UserName = order.User?.Name,
                RestaurantId = order.RestaurantId,
                RestaurantName = order.Restaurant?.Name,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                ShippingAddress = order.ShippingAddress,
                CreatedAt = order.CreatedAt,
                DeliveryPartnerId = order.DeliveryPartnerId,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    MenuId = oi.MenuId,
                    MenuItemName = oi.MenuItem?.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            });
        }
    }
}
