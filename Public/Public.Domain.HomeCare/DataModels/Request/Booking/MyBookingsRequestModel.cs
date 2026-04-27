using Shared.HomeCare.Enums;

namespace Public.Domain.HomeCare.DataModels.Request.Booking
{
    public class MyBookingsRequestModel
    {
        public BookingTab Tab { get; set; } = BookingTab.Upcoming;
    }
}