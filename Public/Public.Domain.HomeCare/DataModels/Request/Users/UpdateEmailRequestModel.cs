namespace Public.Domain.HomeCare.DataModels.Request.Users
{
    public class UpdateEmailRequestModel
    {
        public string NewEmail { get; set; } = string.Empty;

        public string Otp { get; set; } = string.Empty;
    }
}