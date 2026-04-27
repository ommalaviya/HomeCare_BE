using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.SupportTicket;
using Admin.Domain.HomeCare.DataModels.Response.SupportTicket;
using Admin.Domain.HomeCare.Interface;
using AutoMapper;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Services;
using System.Linq.Expressions;
using System.Security.Claims;
using Shared.Extensions;

namespace Admin.Application.HomeCare.Services
{
    public class SupportTicketService(
        ISupportTicketRepository supportTicketRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal)
        : GenericService<SupportTicket>(supportTicketRepository, unitOfWork, mapper, principal),
          ISupportTicketService
    {

        public async Task<DataQueryResponseModel<GetSupportTicketResponseModel>> GetAllSupportTicketsAsync(FilterSupportTicketRequestModel? filter = null)
        {
            filter ??= new FilterSupportTicketRequestModel();

            Expression<Func<SupportTicket, bool>> expression = x => true;

            if (!string.IsNullOrEmpty(filter.UserName))
            {
                expression = expression.Add(x =>
                    x.Name.ToLower().Contains(filter.UserName.ToLower()));
            }

            if (filter.SubmittedAt.HasValue)
            {
                var date = filter.SubmittedAt.Value.Date.ToUniversalTime();

                expression = expression.Add(x =>
                    x.SubmittedAt >= date &&
                    x.SubmittedAt < date.AddDays(1));
            }

            var response = await GetAllAsync(predicate: expression, model: filter);

            return new DataQueryResponseModel<GetSupportTicketResponseModel>
            {
                TotalRecords = response.TotalRecords,
                Records = ToResponseModel<IEnumerable<GetSupportTicketResponseModel>>(response.Records)
            };
        }
    }
}