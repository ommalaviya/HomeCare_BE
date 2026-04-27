using Admin.Domain.HomeCare.DataModels.Request.ServicePartner;
using Admin.Domain.HomeCare.DataModels.Response.ServicePartner;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface IServicePartnerService : IGenericService<ServicePartner>
    {
        Task<FilteredDataQueryResponseModel<GetServicePartnerResponseModel>> GetAllServicePartnersAsync(
             FilterServicePartnerRequestModel? filter = null);

        Task<ServicePartnerDetailResponse> GetDetailAsync(int id);

        Task<IEnumerable<PartnerServiceResponseModel>> GetPartnerServicesAsync(int servicePartnerId);

        Task<ServicePartnerActionResponse> ApproveAsync(int id);

        Task<ServicePartnerActionResponse> RejectAsync(int id, RejectServicePartnerRequestModel request);

        Task<DataQueryResponseModel<AssignedServiceResponse>> GetAssignedServicesAsync(
            int servicePartnerId,
            FilterAssignedServicesRequestModel filter);

        Task<bool> ToggleStatusAsync(int id);

        Task<bool> DeleteServicePartnerAsync(int id);

        Task<FileContentHttpResult> DownloadAttachmentAsync(int servicePartnerId, int attachmentId);
    }
}