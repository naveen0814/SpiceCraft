using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpicecraftAPI.Models
{
    public class DeliveryPartner
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
        public string PasswordHash { get; set; }    // <-- ADD THIS LINE
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
    }
}
