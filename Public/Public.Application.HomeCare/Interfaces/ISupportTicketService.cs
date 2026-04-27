using Public.Domain.HomeCare.DataModels.Request.SupportTicket;
using Public.Domain.HomeCare.DataModels.Response.SupportTicket;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Public.Application.HomeCare.Interfaces
{
    public interface ISupportTicketService : IGenericService<SupportTicket>
    {
        Task<GetSupportTicketResponseModel> CreateSupportTicketAsync(
            CreateSupportTicketRequestModel request);
    }
}