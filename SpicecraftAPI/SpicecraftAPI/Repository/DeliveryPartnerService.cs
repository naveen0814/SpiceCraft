using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Data;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

public class DeliveryPartnerService : IDeliveryPartnerService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public DeliveryPartnerService(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // Helper: Save file and return relative path
    private string SaveFile(IFormFile file)
    {
        if (file == null) return null;
        string uploads = Path.Combine(_env.ContentRootPath, "Uploads/DeliveryPartners");
        Directory.CreateDirectory(uploads);
        string fileName = $"{Guid.NewGuid()}_{file.FileName}";
        string filePath = Path.Combine(uploads, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);
        // Return relative path (useful for frontend)
        return Path.Combine("Uploads/DeliveryPartners", fileName).Replace("\\", "/");
    }

    // Create a new delivery partner application (with user id)
    public async Task<DeliveryPartnerDto> CreateAsync(CreateDeliveryPartnerDto dto, int userId)
    {
        var dp = new DeliveryPartner
        {
            UserId = userId,
            Name = dto.Name,
            PhoneNumber = dto.PhoneNumber,
            PAN = dto.PAN,
            BankDetails = dto.BankDetails,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            DrivingLicensePath = SaveFile(dto.DrivingLicense),
            BikeRCPath = SaveFile(dto.BikeRC),
            PhotoPath = SaveFile(dto.Photo),
            CreatedAt = DateTime.UtcNow,
            IsApproved = false
        };
        _context.DeliveryPartners.Add(dp);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(dp.DeliveryPartnerId);
    }

    public async Task<DeliveryPartnerDto> GetByIdAsync(int id)
    {
        var dp = await _context.DeliveryPartners.FindAsync(id);
        if (dp == null) return null;
        return new DeliveryPartnerDto
        {
            DeliveryPartnerId = dp.DeliveryPartnerId,
            UserId = dp.UserId,
            Name = dp.Name,
            PhoneNumber = dp.PhoneNumber,
            PAN = dp.PAN,
            BankDetails = dp.BankDetails,
            DrivingLicensePath = dp.DrivingLicensePath,
            BikeRCPath = dp.BikeRCPath,
            PhotoPath = dp.PhotoPath,
            IsApproved = dp.IsApproved,
            CreatedAt = dp.CreatedAt
        };
    }

    // Get by UserId (for user to check their application)
    public async Task<DeliveryPartnerDto> GetByUserIdAsync(int userId)
    {
        var dp = await _context.DeliveryPartners
            .FirstOrDefaultAsync(d => d.UserId == userId);
        if (dp == null) return null;
        return await GetByIdAsync(dp.DeliveryPartnerId);
    }

    public async Task<IEnumerable<DeliveryPartnerDto>> GetAllAsync() =>
        await _context.DeliveryPartners
            .Select(dp => new DeliveryPartnerDto
            {
                DeliveryPartnerId = dp.DeliveryPartnerId,
                UserId = dp.UserId,
                Name = dp.Name,
                PhoneNumber = dp.PhoneNumber,
                PAN = dp.PAN,
                BankDetails = dp.BankDetails,
                DrivingLicensePath = dp.DrivingLicensePath,
                BikeRCPath = dp.BikeRCPath,
                PhotoPath = dp.PhotoPath,
                IsApproved = dp.IsApproved,
                CreatedAt = dp.CreatedAt
            }).ToListAsync();

    public async Task<bool> ApproveAsync(int id)
    {
        var dp = await _context.DeliveryPartners.FindAsync(id);
        if (dp == null) return false;
        dp.IsApproved = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AssignedOrderDto>> GetAssignedOrdersAsync(int partnerId)
    {
        // Ensure your Order entity has navigation to User and Restaurant.
        // Adjust property names if different.
        var list = await _context.Orders
            .Where(o => o.DeliveryPartnerId == partnerId
                        && (o.OrderStatus == "Assigned" || o.OrderStatus == "Accepted"))
            .Include(o => o.User)         // navigation to User
            .Include(o => o.Restaurant)   // navigation to Restaurant
            .Select(o => new AssignedOrderDto
            {
                OrderId = o.OrderId,
                PickupLocation = o.Restaurant.Location,
                DeliveryLocation = o.ShippingAddress,
                AssignedAt = o.CreatedAt,
                Status = o.OrderStatus,
                CustomerName = o.User != null ? o.User.Name : null,
                CustomerPhone = o.User != null ? o.User.PhoneNumber : null
            })
            .ToListAsync();
        return list;
    }

    public async Task<bool> AcceptOrderAsync(int partnerId, int orderId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.DeliveryPartnerId == partnerId);
        if (order == null) return false;
        order.OrderStatus = "Accepted";
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DeliveryOrderDto>> GetOrderHistoryAsync(int partnerId)
    {
        return await _context.Orders
            .Where(o => o.DeliveryPartnerId == partnerId && o.OrderStatus == "Delivered")
            .Select(o => new DeliveryOrderDto
            {
                OrderId = o.OrderId,
                PickupLocation = o.Restaurant.Location,
                DeliveryLocation = o.ShippingAddress,
                AssignedAt = o.CreatedAt,
                Status = o.OrderStatus,
                Earnings = o.TotalAmount * 0.1m
            }).ToListAsync();
    }

    public async Task<decimal> GetEarningsAsync(int partnerId)
    {
        return await _context.Orders
            .Where(o => o.DeliveryPartnerId == partnerId && o.OrderStatus == "Delivered")
            .SumAsync(o => o.TotalAmount * 0.1m);
    }

    public async Task<bool> UpdateAssignedOrderStatusAsync(int partnerId, int orderId, string orderStatus)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.DeliveryPartnerId == partnerId);
        if (order == null)
            return false;

        order.OrderStatus = orderStatus;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DeliveryPartnerDto> UpdateAsync(int id, UpdateDeliveryPartnerDto dto)
    {
        var dp = await _context.DeliveryPartners.FindAsync(id);
        if (dp == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name)) dp.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) dp.PhoneNumber = dto.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(dto.PAN)) dp.PAN = dto.PAN;
        if (!string.IsNullOrWhiteSpace(dto.BankDetails)) dp.BankDetails = dto.BankDetails;
        if (!string.IsNullOrWhiteSpace(dto.Password)) dp.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        if (dto.DrivingLicense != null) dp.DrivingLicensePath = SaveFile(dto.DrivingLicense);
        if (dto.BikeRC != null) dp.BikeRCPath = SaveFile(dto.BikeRC);
        if (dto.Photo != null) dp.PhotoPath = SaveFile(dto.Photo);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dp = await _context.DeliveryPartners.FindAsync(id);
        if (dp == null) return false;
        _context.DeliveryPartners.Remove(dp);
        await _context.SaveChangesAsync();
        return true;
    }
}
