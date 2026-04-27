namespace Admin.Domain.HomeCare.DataModels.Request.Admin
{
    public class UpdateAdminPasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}