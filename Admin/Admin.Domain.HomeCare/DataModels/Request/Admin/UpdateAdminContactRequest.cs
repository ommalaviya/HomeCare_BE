namespace Admin.Domain.HomeCare.DataModels.Request.Admin
{
    public class UpdateAdminContactRequest
    {
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? Address { get; set; }
    }
}