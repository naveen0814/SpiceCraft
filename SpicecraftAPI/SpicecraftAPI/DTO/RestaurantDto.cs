using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
namespace SpicecraftAPI.DTO
{
    public class RestaurantDto
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; }
        public string LogoUrl { get; set; }
        public string FssaiLicensePath { get; set; }
        public string GstDocumentPath { get; set; }
    }
    public class CreateRestaurantDto
    {
        [Required] public string Name { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Phone] public string ContactNumber { get; set; }
        public string Location { get; set; }
        public string BankDetails { get; set; }
        public string GSTIN { get; set; }
        public string PANCardNumber { get; set; }

        // Logo as URL text
        [Required, Url] public string LogoUrl { get; set; }

        // Keep as file uploads
        public IFormFile FSSAILicense { get; set; }
        public IFormFile GSTDocument { get; set; }
    }

    public class UpdateRestaurantDto
    {
        public string? Name { get; set; }
        [EmailAddress] public string? Email { get; set; }
        public string? Password { get; set; }
        [Phone] public string? ContactNumber { get; set; }
        public string? Location { get; set; }
        public string? BankDetails { get; set; }
        public string? GSTIN { get; set; }
        public string? PANCardNumber { get; set; }

        // LogoUrl as optional text
        [Url] public string? LogoUrl { get; set; }

        // Keep as file uploads
        public IFormFile? FSSAILicense { get; set; }
        public IFormFile? GSTDocument { get; set; }
    }
}
