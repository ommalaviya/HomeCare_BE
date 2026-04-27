namespace Admin.Domain.HomeCare.DataModels.Response.Booking
{
    /// <summary>
    /// Carries the maximum values for filter sliders so the UI can set
    /// correct upper bounds (max booking count and max booking amount).
    /// </summary>
    public class BookingFilterMetaResponse
    {
        /// <summary>Maximum TotalBookedServices across all grouped rows.</summary>
        public int MaxBookedServices { get; set; }

        /// <summary>Maximum TotalBookingAmount across all grouped rows.</summary>
        public decimal MaxBookingAmount { get; set; }
    }
}