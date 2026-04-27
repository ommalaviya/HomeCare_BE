using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.Enums;

namespace Admin.Domain.HomeCare.DataModels.Request.Booking
{
    public class FilterBookingRequestModel : PageRequest
    {
        public int? ServiceTypeId { get; set; }
        public DateOnly? Date { get; set; }
        public string? Time { get; set; }
        public int? BookedServicesMin { get; set; }
        public int? BookedServicesMax { get; set; }
        public decimal? AmountMin { get; set; }
        public decimal? AmountMax { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public BookingStatus? Status { get; set; }
    }
}