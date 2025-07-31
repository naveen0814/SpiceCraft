using SpicecraftAPI.DTO;

namespace SpicecraftAPI.Interface
{
    public interface ICartService
    {
        Task<IEnumerable<CartDto>> GetAllAsync();
        Task<CartDto> GetByIdAsync(int id);
        Task<CartDto> CreateAsync(CreateCartDto dto);
        Task<CartDto> UpdateAsync(int id, UpdateCartDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CartDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<CartItemDto>> GetItemsByCartIdAsync(int cartId); // Optional extension
    }
}
