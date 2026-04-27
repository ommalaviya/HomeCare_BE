using Shared.HomeCare.DataModel.Request;

namespace Admin.Domain.HomeCare.DataModels.Request.AdminUser
{
    public class FilterAdminUserRequestModel : PageRequest
    {
        public bool? IsSuperAdmin { get; set; }
        public bool? IsActive { get; set; }
    }
}