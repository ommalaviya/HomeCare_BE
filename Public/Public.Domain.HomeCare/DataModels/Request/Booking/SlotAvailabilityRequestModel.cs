using System.Text.Json.Serialization;

namespace Public.Domain.HomeCare.DataModels.Request.Booking
{
    public class SlotAvailabilityRequestModel
    {
        public int ServiceId { get; set; }

        public int ServiceTypeId { get; set; }

        public DateOnly BookingDate { get; set; }

        public string BookingTime { get; set; } = string.Empty;
        
        [JsonIgnore]
        public int? KnownDurationMinutes { get; set; }
    }
}