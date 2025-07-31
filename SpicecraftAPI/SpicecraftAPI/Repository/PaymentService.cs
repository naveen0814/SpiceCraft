using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Data;
using SpicecraftAPI.DTO;
using SpicecraftAPI.Interface;
using SpicecraftAPI.Models;

namespace SpicecraftAPI.Repository
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            return await _context.Payments
                .Select(p => new PaymentDto
                {
                    PaymentId = p.PaymentId,
                    OrderId = p.OrderId,
                    PaymentStatus = p.PaymentStatus,
                    PaymentMode = p.PaymentMode,
                    TransactionId = p.TransactionId,
                    PaymentDate = p.PaymentDate
                }).ToListAsync();
        }

        public async Task<PaymentDto> GetByIdAsync(int id)
        {
            var p = await _context.Payments.FindAsync(id);
            if (p == null) return null;

            return new PaymentDto
            {
                PaymentId = p.PaymentId,
                OrderId = p.OrderId,
                PaymentStatus = p.PaymentStatus,
                PaymentMode = p.PaymentMode,
                TransactionId = p.TransactionId,
                PaymentDate = p.PaymentDate
            };
        }

        public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
        {
            var p = new Payment
            {
                OrderId = dto.OrderId,
                PaymentStatus = dto.PaymentStatus ?? "Completed", // Default to Completed
                PaymentMode = dto.PaymentMode,
                TransactionId = dto.TransactionId,
                PaymentDate = dto.PaymentDate == default ? DateTime.UtcNow : dto.PaymentDate
            };

            _context.Payments.Add(p);

            // Update the linked Order
            var order = await _context.Orders.FindAsync(dto.OrderId);
            if (order != null)
            {
                order.PaymentStatus = "Completed";
                order.OrderStatus = "Confirmed"; // Or your preferred status

                // Assign free delivery partner **after payment completed**
                var freePartner = await _context.DeliveryPartners
                    .Where(dp => dp.IsApproved)
                    .FirstOrDefaultAsync(dp => !_context.Orders.Any(o =>
                        o.DeliveryPartnerId == dp.DeliveryPartnerId &&
                        o.OrderStatus != "Delivered" && o.OrderStatus != "Completed"));

                if (freePartner != null)
                {
                    order.DeliveryPartnerId = freePartner.DeliveryPartnerId;
                    order.OrderStatus = "Assigned";
                }
                else
                {
                    order.DeliveryPartnerId = null;
                    order.OrderStatus = "Waiting for Assignment";
                }
            }

            await _context.SaveChangesAsync();
            return await GetByIdAsync(p.PaymentId);
        }

        public async Task<PaymentDto> UpdateAsync(int id, UpdatePaymentDto dto)
        {
            var p = await _context.Payments.FindAsync(id);
            if (p == null) return null;

            if (dto.OrderId != 0) p.OrderId = dto.OrderId;
            if (!string.IsNullOrWhiteSpace(dto.PaymentStatus)) p.PaymentStatus = dto.PaymentStatus;
            if (!string.IsNullOrWhiteSpace(dto.PaymentMode)) p.PaymentMode = dto.PaymentMode;
            if (!string.IsNullOrWhiteSpace(dto.TransactionId)) p.TransactionId = dto.TransactionId;
            if (dto.PaymentDate != default) p.PaymentDate = dto.PaymentDate;

            // Update the linked Order
            var order = await _context.Orders.FindAsync(p.OrderId);
            if (order != null)
            {
                order.PaymentStatus = "Completed";
                order.OrderStatus = "Confirmed"; // Or your preferred status

                // Assign free delivery partner **after payment completed**
                var freePartner = await _context.DeliveryPartners
                    .Where(dp => dp.IsApproved)
                    .FirstOrDefaultAsync(dp => !_context.Orders.Any(o =>
                        o.DeliveryPartnerId == dp.DeliveryPartnerId &&
                        o.OrderStatus != "Delivered" && o.OrderStatus != "Completed"));

                if (freePartner != null)
                {
                    order.DeliveryPartnerId = freePartner.DeliveryPartnerId;
                    order.OrderStatus = "Assigned";
                }
                else
                {
                    order.DeliveryPartnerId = null;
                    order.OrderStatus = "Waiting for Assignment";
                }
            }

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var p = await _context.Payments.FindAsync(id);
            if (p == null) return false;

            _context.Payments.Remove(p);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
