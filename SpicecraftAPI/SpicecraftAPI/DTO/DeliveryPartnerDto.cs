namespace SpicecraftAPI.DTO
{
    public class DeliveryPartnerDto
    {
        public int DeliveryPartnerId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string PAN { get; set; }
        public string BankDetails { get; set; }
        public string DrivingLicensePath { get; set; }
        public string BikeRCPath { get; set; }
        public string PhotoPath { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateDeliveryPartnerDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string PAN { get; set; }
        public string BankDetails { get; set; }
        public IFormFile DrivingLicense { get; set; }
        public IFormFile BikeRC { get; set; }
        public IFormFile Photo { get; set; }
        public string Password { get; set; }    // <-- Required for registration!
    }

    public class DeliveryOrderDto
    {
        public int OrderId { get; set; }
        public string PickupLocation { get; set; }
        public string DeliveryLocation { get; set; }
        public DateTime AssignedAt { get; set; }
        public string Status { get; set; }
        public decimal Earnings { get; set; }
    }

    public class UpdateDeliveryPartnerDto
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PAN { get; set; }
        public string? BankDetails { get; set; }
        public IFormFile? DrivingLicense { get; set; }
        public IFormFile? BikeRC { get; set; }
        public IFormFile? Photo { get; set; }
        public string? Password { get; set; }
    }
    public class AssignedOrderDto
    {
        public int OrderId { get; set; }
        public string PickupLocation { get; set; }
        public string DeliveryLocation { get; set; }
        public DateTime AssignedAt { get; set; }
        public string Status { get; set; }

        // New fields:
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
    }
}
