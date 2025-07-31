using SpicecraftAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SpicecraftAPI.DTO
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
    }
    public class CreatePaymentDto
    {
        public int OrderId { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class UpdatePaymentDto : CreatePaymentDto
    {
        public int PaymentId { get; set; }
    }
}
