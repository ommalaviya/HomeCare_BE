using AutoMapper;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.SupportTicket;
using Public.Domain.HomeCare.DataModels.Response.SupportTicket;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Services;
using System.Security.Claims;

namespace Public.Application.HomeCare.Services
{
    public class SupportTicketService(
        ISupportTicketRepository supportTicketRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal)
        : GenericService<SupportTicket>(supportTicketRepository, unitOfWork, mapper, principal),
          ISupportTicketService
    {
        public async Task<GetSupportTicketResponseModel> CreateSupportTicketAsync(
            CreateSupportTicketRequestModel request)
        {
            var entity = ToEntity(request);
            entity.SubmittedAt = DateTime.UtcNow;
            var result = await AddAsync(entity);
            return ToResponseModel<GetSupportTicketResponseModel>(result);
        }
    }
}