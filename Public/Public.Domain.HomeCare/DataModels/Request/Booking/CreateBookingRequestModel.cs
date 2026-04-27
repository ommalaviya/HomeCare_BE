namespace Public.Domain.HomeCare.DataModels.Request.Booking
{
    public class CreateBookingRequestModel
    {
        public int ServiceId { get; set; }

        public int ServiceTypeId { get; set; }

        public int AddressId { get; set; }

        public DateOnly BookingDate { get; set; }

        public string BookingTime { get; set; } = string.Empty;

        public int PaymentMethod { get; set; }

        public int? OfferId { get; set; }
    }
}