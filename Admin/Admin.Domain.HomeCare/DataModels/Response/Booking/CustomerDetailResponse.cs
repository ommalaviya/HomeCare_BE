namespace Admin.Domain.HomeCare.DataModels.Response.Customer
{
    public class CustomerDetailResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}