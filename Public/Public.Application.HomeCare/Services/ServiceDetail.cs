using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Response.Home;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;

namespace Public.Application.HomeCare.Services;

public class ServiceDetailService(
    IServiceRepository serviceRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ClaimsPrincipal principal)
    : GenericService<ServicesOfSubCategory>(serviceRepository, unitOfWork, mapper, principal),
      IServiceDetailService
{
    public async Task<ServiceDetailResponseModel> GetServiceDetailAsync(int serviceId)
    {
        //if id is less than or equal to 0
         if (serviceId <= 0)
            throw new KeyNotFoundException(
                string.Format(Messages.GreaterThanZero, Messages.ServiceId));
                
        //Check soft-delete & null
        var service = await GetOrThrowAsync(serviceId,
            string.Format(Messages.NotFound, Messages.Services));

        //IsAvailable checked
        if (!service.IsAvailable)
            throw new KeyNotFoundException(
                string.Format(Messages.Unavailable, Messages.Services));

        var serviceDetail = await FindOrThrowAsync(
            x => x.Id == serviceId,
            string.Format(Messages.NotFound, Messages.Services),
            includer: new ExpressionIncluder<ServicesOfSubCategory>
            {
                ThenIncludes =
                [
                    q => q.Include(x => x.Images),
                    q => q.Include(x => x.ServiceFilters),
                    q => q.Include(x => x.SubCategory)
                             .ThenInclude(sc => sc.Category)
                             .ThenInclude(c => c.ServiceTypes)
                ]
            });

        var relatedResult = await GetAllAsync(
            x => x.SubCategoryId == service.SubCategoryId
              && x.Id != serviceId
              && !x.IsDeleted
              && x.IsAvailable,
            includer: new ExpressionIncluder<ServicesOfSubCategory>
            {
                Includes = [x => x.Images]
            });

        var related = relatedResult.Records.Take(5).ToList();

        var result = mapper.Map<ServiceDetailResponseModel>(serviceDetail);
        result.RelatedServices = mapper.Map<List<ServiceResponseModel>>(related);

        return result;
    }
}