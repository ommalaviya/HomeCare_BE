using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.Enums;

namespace Admin.Domain.HomeCare.DataModels.Request.Booking
{
    public class FilterCustomerBookingsRequestModel : PageRequest
    {
        public int? ServiceTypeId { get; set; }
        public DateOnly? Date { get; set; }
        public string? Time { get; set; }
        public decimal? AmountMin { get; set; }
        public decimal? AmountMax { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public BookingStatus? Status { get; set; }
    }
}