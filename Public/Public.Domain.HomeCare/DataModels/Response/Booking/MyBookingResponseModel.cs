using Shared.HomeCare.Enums;

namespace Public.Domain.HomeCare.DataModels.Response.Booking
{
    public class MyBookingResponseModel
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; } = string.Empty;
        public DateOnly BookingDate { get; set; }
        public string BookingTime { get; set; } = string.Empty;
        public decimal BookingAmount { get; set; }
        public BookingStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public MyBookingAddressModel? Address { get; set; }
        public MyBookingPartnerModel? AssignedPartner { get; set; }
    }
    public class MyBookingAddressModel
    {
        public int AddressId { get; set; }
        public string HouseFlatNumber { get; set; } = string.Empty;
        public string? Landmark { get; set; }
        public string? FullAddress { get; set; }
        public string? SaveAs { get; set; }
    }
    public class MyBookingPartnerModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
    }
}