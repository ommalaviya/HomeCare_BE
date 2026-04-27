namespace Admin.Domain.HomeCare.DataModels.Response.Booking
{
    public class BookingDetailResponse
    {
        public int BookingId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public DateOnly BookingDate { get; set; }
        public string BookingTime { get; set; } = string.Empty;
        public int? AssignedPartnerId { get; set; }
        public string? AssignedExpertName { get; set; }
        public string? AssignedExpertImageUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal BookingAmount { get; set; }

        // Computed flags — UI uses these to show/grey-out action menu items
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