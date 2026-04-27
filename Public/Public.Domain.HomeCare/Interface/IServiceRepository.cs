using Public.Domain.HomeCare.DataModels.Response.Home;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Public.Domain.HomeCare.Interface
{
    public interface IServiceRepository : IGenericRepository<ServicesOfSubCategory>
    {
        Task<List<ServiceWithBookingCount>> GetServicesWithImagesAsync();
        Task<List<ServiceNamesResponseModel>> GetServiceNamesAsync();
    }
}