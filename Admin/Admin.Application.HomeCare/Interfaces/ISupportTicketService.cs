using Admin.Domain.HomeCare.DataModels.Request.SupportTicket;
using Admin.Domain.HomeCare.DataModels.Response.SupportTicket;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface ISupportTicketService : IGenericService<SupportTicket>
    {
        Task<DataQueryResponseModel<GetSupportTicketResponseModel>> GetAllSupportTicketsAsync(FilterSupportTicketRequestModel? filter = null);
    }
}