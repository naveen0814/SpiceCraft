using SpicecraftAPI.Models;
using SpicecraftAPI.DTO;

namespace SpicecraftAPI.Interface
{
    public interface IRestaurantService
    {
        Task<IEnumerable<Restaurant>> GetAllRawAsync();
        Task<Restaurant> GetByIdRawAsync(int id);
        Task<Restaurant> CreateRawAsync(CreateRestaurantDto dto, int userId);
        Task<Restaurant> UpdateRawAsync(int id, UpdateRestaurantDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ApproveAsync(int id);
        Task<RestaurantDto?> UpdateOwnAsync(int restaurantId, UpdateRestaurantDto dto);
        Task<Restaurant> GetRestaurantByUserIdAsync(int userId);
        Task<Restaurant> GetRestaurantEntityByUserIdAsync(int userId);
        Task<IEnumerable<RestaurantDto>> GetByRestaurantIdAsync(int restaurantId);
        Task<IEnumerable<OrderDto>> GetOrdersForRestaurantAsync(int restaurantId);
    }
}