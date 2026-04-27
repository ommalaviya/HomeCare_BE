using System.Security.Claims;
using AutoMapper;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Response.Home;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Services;

public class HomeService(
    IServiceRepository serviceRepository,
    IServiceTypeRepository serviceTypeRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ClaimsPrincipal principal)
    : GenericService<ServicesOfSubCategory>(serviceRepository, unitOfWork, mapper, principal),
      IHomeService
{
    public async Task<List<ServiceNamesResponseModel>> GetServiceNamesAsync()
    {
        return await serviceRepository.GetServiceNamesAsync();
    }

    public async Task<List<ServiceTypeResponseModel>> GetServiceTypesAsync()
    {
        var data = await serviceTypeRepository.GetAllAsync(x => !x.IsDeleted);
        return MapToList<ServiceTypeResponseModel>(data.Records);
    }

    public async Task<List<ServiceResponseModel>> GetPopularServicesAsync()
    {
        var data = await serviceRepository.GetServicesWithImagesAsync();
        return MapToList<ServiceResponseModel>(
            data.OrderByDescending(x => x.TotalBookings).Take(20));
    }

    public async Task<List<ServiceResponseModel>> GetAllServicesAsync()
    {
        var data = await serviceRepository.GetServicesWithImagesAsync();
        return MapToList<ServiceResponseModel>(
        data.OrderBy(x => x.Service.Name));
    }

    public async Task<CountResponseModel> GetCountsAsync()
    {
        var totalServices = await CountAsync(x => !x.IsDeleted);
        var totalUsers = await userRepository.CountAsync(x => !x.IsDeleted);

        return mapper.Map<CountResponseModel>((totalUsers, totalServices));
    }
}