namespace Public.Domain.HomeCare.DataModels.Response.ServiceType
{
    public class ServiceTypeWithBookingCountDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? ImageName { get; set; }
        public int TotalBookings { get; set; }
    }
}