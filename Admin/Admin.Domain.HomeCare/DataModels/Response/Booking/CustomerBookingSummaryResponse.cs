namespace Admin.Domain.HomeCare.DataModels.Response.Booking
{
    public class CustomerBookingSummaryResponse
    {
        public int UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalBookedServices { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal TotalBookingAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}