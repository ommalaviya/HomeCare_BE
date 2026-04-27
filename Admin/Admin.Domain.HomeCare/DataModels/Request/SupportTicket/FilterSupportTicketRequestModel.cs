using Shared.HomeCare.DataModel.Request;

namespace Admin.Domain.HomeCare.DataModels.Request.SupportTicket
{
    public class FilterSupportTicketRequestModel : PageRequest
    {
        public string? UserName { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
