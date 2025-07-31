using SpicecraftAPI.DTO;

namespace SpicecraftAPI.Interface
{
    public interface IMenuCategoryService
    {
        Task<IEnumerable<MenuCategoryDto>> GetAllAsync();
        Task<MenuCategoryDto> GetByIdAsync(int id);
        Task<MenuCategoryDto> CreateAsync(CreateMenuCategoryDto dto);
        Task<MenuCategoryDto> UpdateAsync(int id, UpdateMenuCategoryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
