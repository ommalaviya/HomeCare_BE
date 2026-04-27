using Admin.Domain.HomeCare.DataModels.Request.ServiceTypes;
using Admin.Domain.HomeCare.DataModels.Response.ServiceTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface IServiceTypeService : IGenericService<ServiceTypes>
    {
        Task<DataQueryResponseModel<GetServiceTypeResponseModel>> GetServiceTypesAsync();

        Task<GetServiceTypeResponseModel> GetServiceTypeByIdAsync(int id);

        Task<GetServiceTypeResponseModel> CreateServiceTypeAsync(
            CreateServiceTypeRequestModel request);

        Task<GetServiceTypeResponseModel> UpdateServiceTypeAsync(
            UpdateServiceTypeRequestModel request);

        Task<bool> SoftDeleteServiceTypeAsync(int id);

        Task<FileContentHttpResult> GetServiceTypeImageAsync(int id);
    }
}


