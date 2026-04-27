using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Response.ServiceList;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Services;
using Shared.HomeCare.Resources;
using Microsoft.EntityFrameworkCore;

namespace Public.Application.HomeCare.Services
{
    public class ServiceListService(
        ICategoryRepository categoryRepository,
        ISubCategoryRepository subCategoryRepository,
        IServiceRepository serviceRepository,
        IServiceTypeRepository serviceTypeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal)
        : GenericService<Category>(categoryRepository, unitOfWork, mapper, principal),
          IServiceListService
    {
        public async Task<ServiceTypeWithCategoriesResponseModel> GetCategoriesWithSubCategoriesByServiceTypeIdAsync(
            int serviceTypeId)
        {
            var serviceType = await serviceTypeRepository.FindDataAsync(
                st => st.Id == serviceTypeId && !st.IsDeleted)
                ?? throw new KeyNotFoundException(string.Format(Messages.NotFound, Messages.ServiceType));

            var includer = new ExpressionIncluder<Category>
            {
                ThenIncludes =
                [
                    q => q.Include(c => c.SubCategories)
                          .ThenInclude(sc => sc.Services)
                ]
            };

            var response = await GetAllAsync(
                c => c.ServiceTypeId == serviceTypeId && !c.IsDeleted && c.IsActive,
                null,
                includer);

            // Total count of all active+available services across ALL categories under this serviceType
            var totalServiceCount = response.Records
                .SelectMany(c => c.SubCategories)
                .Where(sc => !sc.IsDeleted && sc.IsActive)
                .SelectMany(sc => sc.Services)
                .Count(s => !s.IsDeleted && s.IsAvailable);

            var categories = response.Records.Select(category =>
            {
                var model = ToResponseModel<CategoryWithSubCategoriesResponseModel>(category);
                return model;
            }).ToList();

            return new ServiceTypeWithCategoriesResponseModel
            {
                ServiceName = serviceType.ServiceName,
                TotalServiceCount = totalServiceCount,
                Categories = categories
            };
        }

        public async Task<ServiceListResponseModel> GetServicesBySubCategoryIdAsync(int subCategoryId)
        {
            var subCategory = await subCategoryRepository.FindDataAsync(
                s => s.Id == subCategoryId && !s.IsDeleted)
                ?? throw new KeyNotFoundException(string.Format(Messages.NotFound, Messages.SubCategory));

            var includer = new ExpressionIncluder<ServicesOfSubCategory>
            {
                Includes = [s => s.Images]
            };

            var response = await serviceRepository.GetAllAsync(
                predicate: s => s.SubCategoryId == subCategoryId && !s.IsDeleted,
                includer: includer);

            return new ServiceListResponseModel
            {
                SubCategoryName = subCategory.SubCategoryName,
                TotalCount = response.TotalRecords,
                Services = MapToList<ServiceListItemResponseModel>(response.Records)
            };
        }

        public async Task<List<ServiceSearchResponseModel>> SearchServicesAsync(
            int serviceTypeId, string? term)
        {
            var includer = new ExpressionIncluder<ServicesOfSubCategory>
            {
                Includes = [s => s.Images, s => s.SubCategory, s => s.SubCategory.Category]
            };

            var response = await serviceRepository.GetAllAsync(
                predicate: s =>
                !s.IsDeleted &&
                s.IsAvailable &&
                !s.SubCategory.IsDeleted &&
                !s.SubCategory.Category.IsDeleted &&
                s.SubCategory.Category.ServiceTypeId == serviceTypeId,
                includer: includer
            );

            var filtered = string.IsNullOrWhiteSpace(term)
                ? response.Records
                : response.Records
                .Where(s => s.Name.Contains(term.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            return mapper.Map<List<ServiceSearchResponseModel>>(filtered);
        }
    }
}

