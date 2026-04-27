namespace Admin.Domain.HomeCare.DataModels.Response.Booking
{
    public class AvailableExpertResponse
    {
        public int PartnerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
    }
}