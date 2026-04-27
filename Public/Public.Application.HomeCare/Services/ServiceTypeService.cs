// Public.Application.HomeCare/Services/ServiceTypeService.cs

using System.Security.Claims;
using AutoMapper;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Response.ServiceType;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Services;

namespace Public.Application.HomeCare.Services
{
    public class ServiceTypeService(
        IServiceTypeRepository serviceTypeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal)
        : GenericService<ServiceTypes>(serviceTypeRepository, unitOfWork, mapper, principal),
          IServiceTypeService
    {
        public async Task<List<ServiceTypeBookingResponseModel>> GetServiceTypesWithBookingCountAsync()
        {
            var data = await serviceTypeRepository.GetServiceTypesWithBookingCountAsync();
            return MapToList<ServiceTypeBookingResponseModel>(data);
        }
    }
}