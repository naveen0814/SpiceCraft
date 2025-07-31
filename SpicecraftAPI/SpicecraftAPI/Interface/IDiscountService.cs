using SpicecraftAPI.DTO;

namespace SpicecraftAPI.Interface
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountDto>> GetAllAsync();
        Task<DiscountDto> GetByIdAsync(int id);
        Task<DiscountDto> CreateAsync(CreateDiscountDto dto);
        Task<DiscountDto> UpdateAsync(int id, UpdateDiscountDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
