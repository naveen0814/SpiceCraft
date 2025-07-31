using SpicecraftAPI.DTO;

namespace SpicecraftAPI.Interface
{
    public interface ICartItemService
    {
        Task<IEnumerable<CartItemDto>> GetAllAsync();
        Task<CartItemDto> GetByIdAsync(int id);
        Task<CartItemDto> CreateAsync(CreateCartItemDto dto);
        Task<CartItemDto> UpdateAsync(int id, UpdateCartItemDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CartItemDto>> GetByUserIdAsync(int userId);
        Task<bool> IsCartOwnedByUserAsync(int cartId, int userId);
        Task<bool> IsCartItemOwnedByUserAsync(int cartItemId, int userId);
    }
}
