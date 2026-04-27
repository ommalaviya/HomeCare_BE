using Admin.Domain.HomeCare.DataModels.Request.ServicePartner;
using Admin.Domain.HomeCare.DataModels.Response.ServicePartner;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Admin.Domain.HomeCare.Interface
{
    public interface IServicePartnerRepository : IGenericRepository<ServicePartner>
    {
        Task<ServicePartnerAttachment?> GetAttachmentAsync(int servicePartnerId, int attachmentId);

        Task<ServicePartnerDetailProjection> GetDetailProjectionAsync(int servicePartnerId);

        Task<(int TotalRecords, IEnumerable<AssignedServiceRow> Rows)> GetAssignedBookingsAsync(
            int servicePartnerId,
            FilterAssignedServicesRequestModel filter);

        Task<IEnumerable<PartnerServiceResponseModel>> GetServicesByPartnerTypeAsync(int servicePartnerId);

        Task<FilteredDataQueryResponse<GetServicePartnerResponseModel>> GetServicePartnersWithMetaAsync(FilterServicePartnerRequestModel filter);

        Task<int> GetPendingBookingCountAsync(int servicePartnerId);

        Task<int> GetOtherActivePartnerCountForServiceTypeAsync(int servicePartnerId);
    }
}