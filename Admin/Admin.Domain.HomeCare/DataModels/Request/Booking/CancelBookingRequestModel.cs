namespace Admin.Domain.HomeCare.DataModels.Request.Booking
{
    public class CancelBookingRequestModel
    {
        public int BookingId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}