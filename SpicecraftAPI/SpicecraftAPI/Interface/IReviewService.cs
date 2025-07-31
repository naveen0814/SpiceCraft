using SpicecraftAPI.DTO;

namespace SpicecraftAPI.Interface
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllAsync();
        Task<ReviewDto> GetByIdAsync(int id);
        Task<ReviewDto> CreateAsync(CreateReviewDto dto);
        Task<ReviewDto> UpdateAsync(int id, UpdateReviewDto dto);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<ReviewDto>> GetByMenuIdAsync(int menuId);
        Task<IEnumerable<ReviewDto>> GetByUserIdAsync(int userId);
    }
}
