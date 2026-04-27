namespace Admin.Domain.HomeCare.DataModels.Response.AdminUser
{
    public class GetAdminUserResponseModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }
}