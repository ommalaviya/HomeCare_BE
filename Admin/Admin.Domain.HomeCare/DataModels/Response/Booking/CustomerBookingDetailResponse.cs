namespace Admin.Domain.HomeCare.DataModels.Response.Booking
{
    public class CustomerBookingDetailResponse
    {
        public int BookingId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public int? ServiceTypeId { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateOnly BookingDate { get; set; }
        public string BookingTime { get; set; } = string.Empty;
        public decimal BookingAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public int? AssignedPartnerId { get; set; }
        public string? AssignedExpertName { get; set; }
        public string? AssignedExpertImageUrl { get; set; }

        public bool CanChangeExpert =>
            Status != nameof(Shared.HomeCare.Enums.BookingStatus.Completed) &&
            Status != nameof(Shared.HomeCare.Enums.BookingStatus.Cancelled);

        public bool CanComplete =>
            Status != nameof(Shared.HomeCare.Enums.BookingStatus.Completed) &&
            Status != nameof(Shared.HomeCare.Enums.BookingStatus.Cancelled);

        public bool CanCancel =>
            Status != nameof(Shared.HomeCare.Enums.BookingStatus.Completed) &&
            Status != nameof(Shared.HomeCare.Enums.BookingStatus.Cancelled);

        public bool CanDelete => true;
    }
}