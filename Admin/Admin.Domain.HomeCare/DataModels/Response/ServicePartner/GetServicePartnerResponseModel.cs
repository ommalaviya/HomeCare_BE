using Shared.HomeCare.Enums;

namespace Admin.Domain.HomeCare.DataModels.Response.ServicePartner
{
    public class GetServicePartnerResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Job { get; set; } = string.Empty;
        public int JobsCompleted { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}