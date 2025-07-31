using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }

        public Order Order { get; set; }
    }
}