using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.Enums;

namespace Admin.Domain.HomeCare.DataModels.Request.ServicePartner
{
    public class FilterAssignedServicesRequestModel : PageRequest
    {
        public DateOnly? Date { get; set; }
        public string? Time { get; set; }
        public BookingStatus? ServiceStatus { get; set; }

    }
}
