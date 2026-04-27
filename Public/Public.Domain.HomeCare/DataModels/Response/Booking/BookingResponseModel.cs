using Shared.HomeCare.Enums;

namespace Public.Domain.HomeCare.DataModels.Response.Booking
{
    public class BookingResponseModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ServiceId { get; set; }

        public int ServiceTypeId { get; set; }
        
        public int DurationMinutes { get; set; }

        public int AddressId { get; set; }

        public BookingAddressModel? Address { get; set; }

        public DateOnly BookingDate { get; set; }

        public string BookingTime { get; set; } = string.Empty;

        public PaymentMethod PaymentMethod { get; set; }

        public decimal BookingAmount { get; set; }

        public BookingStatus Status { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public AssignedPartnerModel? AssignedPartner { get; set; }
    }

    public class BookingAddressModel
    {
        public int AddressId { get; set; }

        public string HouseFlatNumber { get; set; } = string.Empty;

        public string? Landmark { get; set; }

        public string? FullAddress { get; set; }

        public string? SaveAs { get; set; }
    }

    public class AssignedPartnerModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }

        public int TotalJobsCompleted { get; set; }
    }
}