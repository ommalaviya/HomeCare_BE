using Public.Domain.HomeCare.DataModels.Response.ServiceType;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Public.Application.HomeCare.Interfaces
{
    public interface IServiceTypeService : IGenericService<ServiceTypes>
    {
        //Returns service types with total booking count
        Task<List<ServiceTypeBookingResponseModel>> GetServiceTypesWithBookingCountAsync();
    }
}