using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.Enums;

namespace Admin.Domain.HomeCare.DataModels.Request.ServicePartner
{
    public class FilterServicePartnerRequestModel : PageRequest
    {
        public string? ServiceTypeName { get; set; }
        public int? JobsCompletedMin { get; set; }
        public int? JobsCompletedMax { get; set; }
        public ServicePartnerStatus? Status { get; set; }
    }
}