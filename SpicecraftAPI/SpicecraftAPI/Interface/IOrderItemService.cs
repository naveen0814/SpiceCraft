using SpicecraftAPI.DTO;

namespace SpicecraftAPI.Interface
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemDto>> GetAllAsync();
        Task<OrderItemDto> GetByIdAsync(int id);
        Task<OrderItemDto> CreateAsync(CreateOrderItemDto dto);
        Task<OrderItemDto> UpdateAsync(int id, UpdateOrderItemDto dto);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<OrderItemDto>> GetItemsByOrderIdAsync(int orderId);
    }
}
