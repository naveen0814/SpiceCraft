using SpicecraftAPI.DTO;

namespace SpicecraftAPI.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto> UpdateAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteAsync(int id);

        Task<UserDto> GetByEmailAsync(string email); // Optional extension
    }
}
