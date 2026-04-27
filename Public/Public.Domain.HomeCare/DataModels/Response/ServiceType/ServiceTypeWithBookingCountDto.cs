namespace Public.Domain.HomeCare.DataModels.Response.ServiceType
{
    public class ServiceTypeBookingResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Image { get; set; }
        public int TotalBookings { get; set; }
    }
}