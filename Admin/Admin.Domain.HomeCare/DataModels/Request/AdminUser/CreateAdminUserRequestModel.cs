namespace Admin.Domain.HomeCare.DataModels.Request.AdminUser
{
    public class CreateAdminUserRequestModel
    {
        public string? Name { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}