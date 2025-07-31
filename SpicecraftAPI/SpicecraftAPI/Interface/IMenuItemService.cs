using SpicecraftAPI.DTO;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Interface
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemDto>> GetAllAsync();
        Task<MenuItemDto> GetByIdAsync(int id);
        Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto);
        Task<MenuItemDto> UpdateAsync(int id, UpdateMenuItemDto dto);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<MenuItemDto>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<MenuItemDto>> GetByRestaurantIdAsync(int restaurantId);
        Task<IEnumerable<MenuItemDto>> SearchAsync(string query);
        Task<IEnumerable<MenuItemDto>> SearchByRestaurantAsync(int restaurantId, string query);



    }
}
