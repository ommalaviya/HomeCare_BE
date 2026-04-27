namespace Admin.Domain.HomeCare.DataModels.Request.AdminUser
{
    public class ChangeAdminUserPasswordRequestModel
    {
        public int TargetAdminId { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}