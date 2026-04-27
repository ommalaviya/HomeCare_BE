using Shared.HomeCare.Enums;

namespace Admin.Domain.HomeCare.DataModels.Response.ServicePartner
{
    public class AssignedServiceResponse
    {
        public int BookingId { get; set; }

        public int ServiceId { get; set; }

        public string ServiceName { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string DateAndTime { get; set; } = string.Empty;

        public string ServiceAddress { get; set; } = string.Empty;

        public string ServiceStatus { get; set; } = string.Empty;
    }
}
