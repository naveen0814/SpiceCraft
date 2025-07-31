using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Data;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class RestaurantService : IRestaurantService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public RestaurantService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IEnumerable<Restaurant>> GetAllRawAsync()
        {
            return await _context.Restaurants.ToListAsync();
        }

        public async Task<Restaurant> GetByIdRawAsync(int id)
        {
            return await _context.Restaurants.FindAsync(id);
        }

        // Instead of GetCurrentUserId(), pass userId as an argument
        public async Task<bool> IsMenuItemOwnedByRestaurant(int userId, int menuItemId)
        {
            var restaurant = await GetRestaurantEntityByUserIdAsync(userId);
            if (restaurant == null) return false;
            var menuItem = await _context.MenuItems.FindAsync(menuItemId);
            if (menuItem == null) return false;
            return menuItem.RestaurantId == restaurant.RestaurantId;
        }

        public async Task<Restaurant> CreateRawAsync(CreateRestaurantDto dto, int userId)
        {
            // Keep file handling for PDF docs
            string fssaiPath = null, gstPath = null;
            // existing PDF upload code...
            var docFolder = Path.Combine(_env.ContentRootPath, "Uploads", "RestaurantDocs");
            Directory.CreateDirectory(docFolder);
            if (dto.FSSAILicense?.FileName.EndsWith(".pdf") == true)
            {
                fssaiPath = Path.Combine(docFolder, $"{Guid.NewGuid()}_{dto.FSSAILicense.FileName}");
                using var s1 = new FileStream(fssaiPath, FileMode.Create);
                await dto.FSSAILicense.CopyToAsync(s1);
            }
            if (dto.GSTDocument?.FileName.EndsWith(".pdf") == true)
            {
                gstPath = Path.Combine(docFolder, $"{Guid.NewGuid()}_{dto.GSTDocument.FileName}");
                using var s2 = new FileStream(gstPath, FileMode.Create);
                await dto.GSTDocument.CopyToAsync(s2);
            }

            var r = new Restaurant
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                ContactNumber = dto.ContactNumber,
                Location = dto.Location,
                BankDetails = dto.BankDetails,
                GSTIN = dto.GSTIN,
                PANCardNumber = dto.PANCardNumber,
                FSSAILicensePath = fssaiPath,
                GSTDocumentPath = gstPath,

                // Assign LogoUrl directly
                LogoPath = dto.LogoUrl,

                CreatedAt = DateTime.UtcNow,
                IsApproved = false,
                UserId = userId
            };

            _context.Restaurants.Add(r);
            await _context.SaveChangesAsync();
            return r;
        }


        public async Task<Restaurant> UpdateRawAsync(int id, UpdateRestaurantDto dto)
        {
            var r = await _context.Restaurants.FindAsync(id);
            if (r == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                r.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Email))
                r.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Password))
                r.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            if (!string.IsNullOrWhiteSpace(dto.ContactNumber))
                r.ContactNumber = dto.ContactNumber;
            if (!string.IsNullOrWhiteSpace(dto.Location))
                r.Location = dto.Location;
            if (!string.IsNullOrWhiteSpace(dto.BankDetails))
                r.BankDetails = dto.BankDetails;
            if (!string.IsNullOrWhiteSpace(dto.GSTIN))
                r.GSTIN = dto.GSTIN;
            if (!string.IsNullOrWhiteSpace(dto.PANCardNumber))
                r.PANCardNumber = dto.PANCardNumber;

            // handle Logo update via URL
            if (!string.IsNullOrWhiteSpace(dto.LogoUrl))
                r.LogoPath = dto.LogoUrl;

            // handle FSSAI/GST updates (unchanged)
            var docFolder = Path.Combine(_env.ContentRootPath, "Uploads", "RestaurantDocs");
            Directory.CreateDirectory(docFolder);
            if (dto.FSSAILicense?.FileName.EndsWith(".pdf") == true)
            {
                var p = Path.Combine(docFolder, $"{Guid.NewGuid()}_{dto.FSSAILicense.FileName}");
                using var s1 = new FileStream(p, FileMode.Create);
                await dto.FSSAILicense.CopyToAsync(s1);
                r.FSSAILicensePath = p;
            }
            if (dto.GSTDocument?.FileName.EndsWith(".pdf") == true)
            {
                var p = Path.Combine(docFolder, $"{Guid.NewGuid()}_{dto.GSTDocument.FileName}");
                using var s2 = new FileStream(p, FileMode.Create);
                await dto.GSTDocument.CopyToAsync(s2);
                r.GSTDocumentPath = p;
            }

            await _context.SaveChangesAsync();
            return r;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var r = await _context.Restaurants.FindAsync(id);
            if (r == null) return false;
            _context.Restaurants.Remove(r);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveAsync(int id)
        {
            var r = await _context.Restaurants.FindAsync(id);
            if (r == null) return false;
            r.IsApproved = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Restaurant> GetRestaurantByUserIdAsync(int userId)
        {
            return await _context.Restaurants.FirstOrDefaultAsync(r => r.UserId == userId);
        }
        public async Task<RestaurantDto?> UpdateOwnAsync(int restaurantId, UpdateRestaurantDto dto)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                restaurant.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                restaurant.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.ContactNumber))
                restaurant.ContactNumber = dto.ContactNumber;

            await _context.SaveChangesAsync();

            // Manual mapping to DTO
            return new RestaurantDto
            {
                RestaurantId = restaurant.RestaurantId,
                Name = restaurant.Name,
                Email = restaurant.Email,
                ContactNumber = restaurant.ContactNumber,
                Location = restaurant.Location,
                CreatedAt = restaurant.CreatedAt,
                IsApproved = restaurant.IsApproved
            };
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersForRestaurantAsync(int restaurantId)
        {
            var orders = await _context.Orders
                .Where(o => o.RestaurantId == restaurantId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    RestaurantId = o.RestaurantId,
                    CreatedAt = o.CreatedAt,
                    OrderStatus = o.OrderStatus,
                    TotalAmount = o.TotalAmount,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        MenuId = oi.MenuId,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        MenuItemName = oi.MenuItem.Name
                    }).ToList()
                })
                .ToListAsync();

            return orders;
        }

        public async Task<Restaurant> GetRestaurantEntityByUserIdAsync(int userId)
        {
            return await _context.Restaurants.FirstOrDefaultAsync(r => r.UserId == userId);
        }
        public async Task<IEnumerable<RestaurantDto>> GetByRestaurantIdAsync(int restaurantId)
        {
            var result = await _context.Restaurants
                .Where(r => r.RestaurantId == restaurantId)
                .Select(r => new RestaurantDto
                {
                    RestaurantId = r.RestaurantId,
                    Name = r.Name,
                    Email = r.Email,
                    ContactNumber = r.ContactNumber,
                    Location = r.Location
                    // Add other fields as needed
                })
                .ToListAsync();

            return result;
        }
    }
}
