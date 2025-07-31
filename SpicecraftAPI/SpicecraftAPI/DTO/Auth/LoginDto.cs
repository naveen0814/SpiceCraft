namespace SpicecraftAPI.DTO.Auth
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class RestaurantLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class DeliveryPartnerLoginDto
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}