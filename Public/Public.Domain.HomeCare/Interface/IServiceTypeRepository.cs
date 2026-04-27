// Public.Domain.HomeCare/Interface/IServiceTypeRepository.cs
using Public.Domain.HomeCare.DataModels.Response.ServiceType;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Public.Domain.HomeCare.Interface
{
    public interface IServiceTypeRepository : IGenericRepository<ServiceTypes>
    {
        Task<List<ServiceTypeWithBookingCountDto>> GetServiceTypesWithBookingCountAsync();
    }
}