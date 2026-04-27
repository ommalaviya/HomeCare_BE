using Public.Domain.HomeCare.DataModels.Response.Home;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Public.Application.HomeCare.Interfaces
{
    public interface IServiceDetailService : IGenericService<ServicesOfSubCategory>
    {
        Task<ServiceDetailResponseModel> GetServiceDetailAsync(int serviceId);
    }
}
