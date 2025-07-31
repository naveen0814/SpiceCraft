using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Data;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _context;

        public MenuItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItemDto>> GetAllAsync()
        {
            return await _context.MenuItems
                .Include(mi => mi.Category)
                .Include(mi => mi.Restaurant)
                .Select(mi => new MenuItemDto
                {
                    MenuId = mi.MenuId,
                    RestaurantId = mi.RestaurantId,
                    RestaurantName = mi.Restaurant.Name,
                    Name = mi.Name,
                    Description = mi.Description,
                    CategoryId = mi.CategoryId,
                    CategoryName = mi.Category.Name,
                    Price = mi.Price,
                    AvailableTime = mi.AvailableTime,
                    IsAvailable = mi.IsAvailable,
                    DietType = mi.DietType,
                    TasteInfo = mi.TasteInfo,
                    Calories = mi.Calories,
                    Fats = mi.Fats,
                    Proteins = mi.Proteins,
                    Carbohydrates = mi.Carbohydrates,
                    ImageUrl = mi.ImageUrl
                }).ToListAsync();
        }

        public async Task<MenuItemDto> GetByIdAsync(int id)
        {
            var item = await _context.MenuItems
                .Include(mi => mi.Category)
                .Include(mi => mi.Restaurant)
                .FirstOrDefaultAsync(mi => mi.MenuId == id);

            if (item == null) return null;

            return new MenuItemDto
            {
                MenuId = item.MenuId,
                RestaurantId = item.RestaurantId,
                RestaurantName = item.Restaurant.Name,
                Name = item.Name,
                Description = item.Description,
                CategoryId = item.CategoryId,
                CategoryName = item.Category.Name,
                Price = item.Price,
                AvailableTime = item.AvailableTime,
                IsAvailable = item.IsAvailable,
                DietType = item.DietType,
                TasteInfo = item.TasteInfo,
                Calories = item.Calories,
                Fats = item.Fats,
                Proteins = item.Proteins,
                Carbohydrates = item.Carbohydrates,
                ImageUrl = item.ImageUrl
            };
        }

        public async Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto)
        {
            // Prevent adding items to a non-existent or unapproved restaurant
            var restaurant = await _context.Restaurants.FindAsync(dto.RestaurantId);
            if (restaurant == null || !restaurant.IsApproved) return null;

            var item = new MenuItem
            {
                RestaurantId = dto.RestaurantId,
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Price = dto.Price,
                AvailableTime = dto.AvailableTime,
                IsAvailable = dto.IsAvailable,
                DietType = dto.DietType,
                TasteInfo = dto.TasteInfo,
                Calories = dto.Calories,
                Fats = dto.Fats,
                Proteins = dto.Proteins,
                Carbohydrates = dto.Carbohydrates,
                ImageUrl = dto.ImageUrl
            };

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(item.MenuId);
        }

        public async Task<MenuItemDto> UpdateAsync(int id, UpdateMenuItemDto dto)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return null;

            if (dto.RestaurantId.HasValue && dto.RestaurantId.Value != item.RestaurantId)
                return null; // Prevent changing ownership

            if (!string.IsNullOrEmpty(dto.Name)) item.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description)) item.Description = dto.Description;
            if (dto.CategoryId.HasValue) item.CategoryId = dto.CategoryId.Value;
            if (dto.Price.HasValue) item.Price = dto.Price.Value;
            if (!string.IsNullOrEmpty(dto.AvailableTime)) item.AvailableTime = dto.AvailableTime;
            if (dto.IsAvailable.HasValue) item.IsAvailable = dto.IsAvailable.Value;
            if (!string.IsNullOrEmpty(dto.DietType)) item.DietType = dto.DietType;
            if (!string.IsNullOrEmpty(dto.TasteInfo)) item.TasteInfo = dto.TasteInfo;
            if (dto.Calories.HasValue) item.Calories = dto.Calories.Value;
            if (dto.Fats.HasValue) item.Fats = dto.Fats.Value;
            if (dto.Proteins.HasValue) item.Proteins = dto.Proteins.Value;
            if (dto.Carbohydrates.HasValue) item.Carbohydrates = dto.Carbohydrates.Value;
            if (!string.IsNullOrEmpty(dto.ImageUrl)) item.ImageUrl = dto.ImageUrl;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return false;

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MenuItemDto>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.MenuItems
                .Where(mi => mi.CategoryId == categoryId)
                .Include(mi => mi.Restaurant)
                .Include(mi => mi.Category)
                .Select(mi => new MenuItemDto
                {
                    MenuId = mi.MenuId,
                    RestaurantId = mi.RestaurantId,
                    RestaurantName = mi.Restaurant.Name,
                    Name = mi.Name,
                    Description = mi.Description,
                    CategoryId = mi.CategoryId,
                    CategoryName = mi.Category.Name,
                    Price = mi.Price,
                    AvailableTime = mi.AvailableTime,
                    IsAvailable = mi.IsAvailable,
                    DietType = mi.DietType,
                    TasteInfo = mi.TasteInfo,
                    Calories = mi.Calories,
                    Fats = mi.Fats,
                    Proteins = mi.Proteins,
                    Carbohydrates = mi.Carbohydrates,
                    ImageUrl = mi.ImageUrl
                }).ToListAsync();
        }

        public async Task<IEnumerable<MenuItemDto>> GetByRestaurantIdAsync(int restaurantId)
        {
            return await _context.MenuItems
                .Where(mi => mi.RestaurantId == restaurantId)
                .Include(mi => mi.Restaurant)
                .Include(mi => mi.Category)
                .Select(mi => new MenuItemDto
                {
                    MenuId = mi.MenuId,
                    RestaurantId = mi.RestaurantId,
                    RestaurantName = mi.Restaurant.Name,
                    Name = mi.Name,
                    Description = mi.Description,
                    CategoryId = mi.CategoryId,
                    CategoryName = mi.Category.Name,
                    Price = mi.Price,
                    AvailableTime = mi.AvailableTime,
                    IsAvailable = mi.IsAvailable,
                    DietType = mi.DietType,
                    TasteInfo = mi.TasteInfo,
                    Calories = mi.Calories,
                    Fats = mi.Fats,
                    Proteins = mi.Proteins,
                    Carbohydrates = mi.Carbohydrates,
                    ImageUrl = mi.ImageUrl
                }).ToListAsync();
        }
        public async Task<IEnumerable<MenuItemDto>> SearchAsync(string query)
        {
            return await _context.MenuItems
                .Include(mi => mi.Category)
                .Include(mi => mi.Restaurant)
                .Where(mi =>
                    mi.Name.Contains(query) ||
                    mi.Description.Contains(query) ||
                    mi.Category.Name.Contains(query) ||
                    mi.Restaurant.Name.Contains(query)
                )
                .Select(mi => new MenuItemDto
                {
                    MenuId = mi.MenuId,
                    RestaurantId = mi.RestaurantId,
                    RestaurantName = mi.Restaurant.Name,
                    Name = mi.Name,
                    Description = mi.Description,
                    CategoryId = mi.CategoryId,
                    CategoryName = mi.Category.Name,
                    Price = mi.Price,
                    AvailableTime = mi.AvailableTime,
                    IsAvailable = mi.IsAvailable,
                    DietType = mi.DietType,
                    TasteInfo = mi.TasteInfo,
                    Calories = mi.Calories,
                    Fats = mi.Fats,
                    Proteins = mi.Proteins,
                    Carbohydrates = mi.Carbohydrates,
                    ImageUrl = mi.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItemDto>> SearchByRestaurantAsync(int restaurantId, string query)
        {
            return await _context.MenuItems
                .Include(mi => mi.Category)
                .Include(mi => mi.Restaurant)
                .Where(mi =>
                    mi.RestaurantId == restaurantId &&
                    (
                      mi.Name.Contains(query) ||
                      mi.Description.Contains(query) ||
                      mi.Category.Name.Contains(query)
                    )
                )
                .Select(mi => new MenuItemDto
                {
                    MenuId = mi.MenuId,
                    RestaurantId = mi.RestaurantId,
                    RestaurantName = mi.Restaurant.Name,
                    Name = mi.Name,
                    Description = mi.Description,
                    CategoryId = mi.CategoryId,
                    CategoryName = mi.Category.Name,
                    Price = mi.Price,
                    AvailableTime = mi.AvailableTime,
                    IsAvailable = mi.IsAvailable,
                    DietType = mi.DietType,
                    TasteInfo = mi.TasteInfo,
                    Calories = mi.Calories,
                    Fats = mi.Fats,
                    Proteins = mi.Proteins,
                    Carbohydrates = mi.Carbohydrates,
                    ImageUrl = mi.ImageUrl
                })
                .ToListAsync();
        }
    }

}
