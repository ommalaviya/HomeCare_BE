namespace Public.Domain.HomeCare.DataModels.Request.Users
{
    public class VerifyOtpRequestModel
    {
        public string Email { get; set; } = string.Empty;

        public string Otp { get; set; } = string.Empty;

        public string? Name { get; set; }
    }
}