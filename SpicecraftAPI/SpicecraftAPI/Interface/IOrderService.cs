using SpicecraftAPI.DTO;

namespace SpicecraftAPI.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto> GetByIdAsync(int id);
        Task<OrderDto> CreateAsync(CreateOrderDto dto);
        Task<OrderDto> UpdateAsync(int id, UpdateOrderDto dto);
        Task<bool> DeleteAsync(int id);
        Task<OrderResultDto> PlaceOrderFromCartAsync(int userId);
        Task<IEnumerable<OrderDto>> GetByUserIdAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string orderStatus);

        Task<IEnumerable<OrderDto>> GetOrdersForRestaurantAsync(int restaurantId);
    }

}
