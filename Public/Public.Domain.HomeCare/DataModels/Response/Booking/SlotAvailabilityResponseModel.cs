namespace Public.Domain.HomeCare.DataModels.Response.Booking
{
    public class SlotAvailabilityResponseModel
    {
        public bool IsAvailable { get; set; }

        public string? Message { get; set; }

        public AssignedPartnerModel? Partner { get; set; }
    }
}