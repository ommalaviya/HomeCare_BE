namespace Shared.HomeCare.DataModel.Request
{
    public class BookingNotifyRequest
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}