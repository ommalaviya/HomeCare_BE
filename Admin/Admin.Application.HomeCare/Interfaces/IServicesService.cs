using Admin.Domain.HomeCare.DataModels.Request.Services;
using Admin.Domain.HomeCare.DataModels.Response.Services;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface IServicesService : IGenericService<ServicesOfSubCategory>
    {
        Task<FilteredDataQueryResponseModel<GetServicesListResponseModel>> GetServicesBySubCategoryAsync(FilterServicesRequestModel request);

        Task<GetServiceByIdResponseModel> GetServiceByIdAsync(int id);

        Task<GetServiceByIdResponseModel> CreateServiceAsync(CreateServiceRequestModel request);

        Task<GetServiceByIdResponseModel> UpdateServiceAsync(UpdateServiceRequestModel request);

        Task<bool> DeleteServiceAsync(int id);

        Task<bool> ToggleAvailabilityAsync(int id, bool isAvailable);
        Task<ServiceTypeFullDataResponseModel> GetAllByServiceTypeAsync(int serviceTypeId);
    }
}