using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Data;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    Gender = u.Gender,
                    Role = u.Role.ToString(),
                    CreatedAt = u.CreatedAt
                }).ToListAsync();
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return null;

            return new UserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                Gender = u.Gender,
                Role = u.Role.ToString(),
                CreatedAt = u.CreatedAt
            };
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var u = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash), // hash here
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Gender = dto.Gender,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(u);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(u.UserId);
        }
        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return null;

            existingUser.Name = dto.Name ?? existingUser.Name;
            existingUser.Email = dto.Email ?? existingUser.Email;
            existingUser.PhoneNumber = dto.PhoneNumber ?? existingUser.PhoneNumber;
            existingUser.Address = dto.Address ?? existingUser.Address;
            existingUser.Gender = dto.Gender ?? existingUser.Gender;

            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);

            if (dto.Role.HasValue)
                existingUser.Role = dto.Role.Value;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id); // return updated data
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return false;

            _context.Users.Remove(u);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDto> GetByEmailAsync(string email)
        {
            var u = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (u == null) return null;

            return new UserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                Gender = u.Gender,
                Role = u.Role.ToString(),
                CreatedAt = u.CreatedAt
            };
        }
    }
}