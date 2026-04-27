namespace Admin.Domain.HomeCare.DataModels.Request.Booking
{
    public class ChangeExpertRequestModel
    {
        public int BookingId { get; set; }
        public int NewPartnerId { get; set; }
    }
}