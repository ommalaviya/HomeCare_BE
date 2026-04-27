namespace Admin.Domain.HomeCare.DataModels.Response.Customer
{
    public class GetCustomerResponseModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? MobileNumber { get; set; }

        public string? Email { get; set; }

        public int PendingBookings { get; set; }

        public int TotalBookings { get; set; }

        public string Status { get; set; }
    }
}