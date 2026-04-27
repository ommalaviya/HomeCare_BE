namespace Public.Domain.HomeCare.DataModels.Request.Payment
{
    public class CreateTransactionIntentRequest
    {
        public int ServiceId { get; set; }
        public int ServiceTypeId { get; set; }
        public int AddressId { get; set; }
        public DateOnly BookingDate { get; set; }
        public string BookingTime { get; set; } = string.Empty;
        public int? OfferId { get; set; }
    }
}