using Shared.HomeCare.Entities;

namespace Public.Domain.HomeCare.DataModels.Response.Home
{
    public class ServiceWithBookingCount
    {
        public ServicesOfSubCategory Service { get; set; } = null!;
        public int TotalBookings { get; set; }
    }
}