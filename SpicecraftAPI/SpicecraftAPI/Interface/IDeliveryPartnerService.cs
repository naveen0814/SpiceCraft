using SpicecraftAPI.DTO;

namespace SpicecraftAPI.Interface
{
    public interface IDeliveryPartnerService
    {
        Task<IEnumerable<DeliveryPartnerDto>> GetAllAsync();
        Task<DeliveryPartnerDto> GetByIdAsync(int id);
        Task<DeliveryPartnerDto> CreateAsync(CreateDeliveryPartnerDto dto, int userId); // <-- add userId here
        Task<bool> ApproveAsync(int id);
        Task<IEnumerable<AssignedOrderDto>> GetAssignedOrdersAsync(int partnerId);
        Task<bool> AcceptOrderAsync(int partnerId, int orderId);
        Task<IEnumerable<DeliveryOrderDto>> GetOrderHistoryAsync(int partnerId);
        Task<decimal> GetEarningsAsync(int partnerId);
        Task<DeliveryPartnerDto> UpdateAsync(int id, UpdateDeliveryPartnerDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAssignedOrderStatusAsync(int partnerId, int orderId, string orderStatus);
        Task<DeliveryPartnerDto> GetByUserIdAsync(int userId); // (for user applications)
    }
}
