using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Services;
using Admin.Domain.HomeCare.DataModels.Response.Services;
using Admin.Domain.HomeCare.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class ServicesService : GenericService<ServicesOfSubCategory>, IServicesService
    {
        private const string SubFolder = "Services";
        private readonly IServicesRepository _repository;
        private readonly IServicesImagesRepository _imagesRepository;
        private readonly IServiceInclusionExclusionRepository _filtersRepository;
        private readonly IFileService _fileService;
        private readonly IBookingRepository _bookingRepository;

        public ServicesService(
            IServicesRepository repository,
            IServicesImagesRepository imagesRepository,
            ClaimsPrincipal principal,
            IServiceInclusionExclusionRepository filtersRepository,
            IFileService fileService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IBookingRepository bookingRepository
            )
            : base(repository, unitOfWork, mapper, principal)
        {
            _repository = repository;
            _imagesRepository = imagesRepository;
            _filtersRepository = filtersRepository;
            _fileService = fileService;
            _bookingRepository = bookingRepository;
        }

        public async Task<FilteredDataQueryResponseModel<GetServicesListResponseModel>> GetServicesBySubCategoryAsync(FilterServicesRequestModel request)
        {
            var includer = new ExpressionIncluder<ServicesOfSubCategory>
            {
                Includes = [x => x.SubCategory]
            };

            var maxPrice = await _repository.GetMaxPriceBySubCategoryAsync(request.SubCategoryId);

            Expression<Func<ServicesOfSubCategory, bool>> predicate = x =>
                x.SubCategoryId == request.SubCategoryId &&
                !x.IsDeleted &&
                (request.FilterSubCategoryId == null || x.SubCategoryId == request.FilterSubCategoryId) &&
                (request.MinPrice == null || x.Price >= request.MinPrice) &&
                (request.MaxPrice == null || x.Price <= request.MaxPrice) &&
                (request.IsAvailable == null || x.IsAvailable == request.IsAvailable) &&
                (request.Commission == null || x.Commission == request.Commission);

            var response = await GetAllAsync(predicate, request, includer);

            return new FilteredDataQueryResponseModel<GetServicesListResponseModel>
            {
                TotalRecords = response.TotalRecords,
                Records = ToResponseModel<IEnumerable<GetServicesListResponseModel>>(response.Records),
                FilterMeta = new FilterRangeMeta { MaxAmount = maxPrice }
            };
        }

        public async Task<GetServiceByIdResponseModel> GetServiceByIdAsync(int id)
        {
            var includer = new ExpressionIncluder<ServicesOfSubCategory>
            {
                Includes = [x => x.SubCategory, x => x.Images, x => x.ServiceFilters],
                ThenIncludes =
                [
                    q => q.Include(x => x.SubCategory)
                           .ThenInclude(sc => sc.Category)
                           .ThenInclude(c => c.ServiceTypes)
                ]
            };

            var entity = await FindOrThrowAsync(
                          x => x.Id == id && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Services),
            includer);

            return ToResponseModel<GetServiceByIdResponseModel>(entity);
        }

        public async Task<GetServiceByIdResponseModel> CreateServiceAsync(CreateServiceRequestModel request)
        {
            await ThrowIfDuplicateAsync(
                x => x.SubCategoryId == request.SubCategoryId &&
                     x.Name == request.Name &&
                     !x.IsDeleted,
                string.Format(Messages.DuplicateRecord, Messages.Services));

            var service = ToEntity(request);
            await AddAsync(service);

            if (request.Images is { Count: > 0 })
            {
                var imageNames = await Task.WhenAll(
                    request.Images
                        .Where(f => f.Length > 0)
                        .Select(f => _fileService.SaveImageAsync(f, SubFolder)));

                await AddRangeAsync(_imagesRepository, imageNames.Select(imageName => new ServicesImages
                {
                    ServiceId = service.Id,
                    ImageName = imageName,
                }));
            }

            await SaveFiltersAsync(service.Id, request.InclusionItems, ServiceInclusionExclusionType.Inclusion);
            await SaveFiltersAsync(service.Id, request.ExclusionItems, ServiceInclusionExclusionType.Exclusion);

            await UnitOfWork.SaveChangesAsync();
            return await GetServiceByIdAsync(service.Id);
        }

        public async Task<GetServiceByIdResponseModel> UpdateServiceAsync(UpdateServiceRequestModel request)
        {
            var includer = new ExpressionIncluder<ServicesOfSubCategory>
            {
                Includes = [x => x.SubCategory, x => x.Images, x => x.ServiceFilters]
            };

            var entity = await FindOrThrowAsync(
                x => x.Id == request.Id && !x.IsDeleted,
                 string.Format(Messages.NotFound, Messages.Services),
             includer);

            await ThrowIfDuplicateAsync(
                x => x.Id != request.Id &&
                     x.SubCategoryId == request.SubCategoryId &&
                     x.Name == request.Name &&
                     !x.IsDeleted,
                string.Format(Messages.DuplicateRecord, Messages.Services));

            Map<ServicesOfSubCategory, UpdateServiceRequestModel>(request, entity);

            if (request.DeleteImageIds is { Count: > 0 })
            {
                var toDelete = entity.Images
                    .Where(i => request.DeleteImageIds.Contains(i.Id))
                    .ToList();

                toDelete.ForEach(img => _fileService.DeleteImage(img.ImageName, SubFolder));
                await RemoveRangeAsync(_imagesRepository, toDelete);
            }

            if (request.NewImages is { Count: > 0 })
            {
                var imageNames = await Task.WhenAll(
                    request.NewImages
                        .Where(f => f.Length > 0)
                        .Select(f => _fileService.SaveImageAsync(f, SubFolder)));

                await AddRangeAsync(_imagesRepository, imageNames.Select(imageName => new ServicesImages
                {
                    ServiceId = entity.Id,
                    ImageName = imageName,
                }));
            }

            await RemoveRangeAsync(_filtersRepository, entity.ServiceFilters.ToList());

            await SaveFiltersAsync(entity.Id, request.InclusionItems, ServiceInclusionExclusionType.Inclusion);
            await SaveFiltersAsync(entity.Id, request.ExclusionItems, ServiceInclusionExclusionType.Exclusion);

            await UpdateAsync(entity);
            return await GetServiceByIdAsync(entity.Id);
        }

        public async Task<bool> ToggleAvailabilityAsync(int id, bool isAvailable)
        {
            var entity = await FindOrThrowAsync(
                 x => x.Id == id && !x.IsDeleted,
             string.Format(Messages.NotFound, Messages.Services));

            entity.IsAvailable = isAvailable;
            await UpdateAsync(entity);

            return true;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var includer = new ExpressionIncluder<ServicesOfSubCategory>
            {
                Includes = [x => x.Images, x => x.ServiceFilters]
            };

            var entity = await FindOrThrowAsync(
                x => x.Id == id && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Services),
             includer);

            if (await _bookingRepository.HasActiveBookingsForServiceAsync(id))
                throw new InvalidOperationException(string.Format(Messages.CannotDeleteActiveBookings,Messages.Services));

            foreach (var img in entity.Images.ToList())
            {
                _fileService.DeleteImage(img.ImageName, SubFolder);
                await _imagesRepository.RemoveAsync(img);
            }

            foreach (var filter in entity.ServiceFilters.ToList())
                await _filtersRepository.RemoveAsync(filter);

            await SoftDeleteAsync(entity);
            return true;
        }

public async Task<ServiceTypeFullDataResponseModel> GetAllByServiceTypeAsync(int serviceTypeId)
{
    var categories = await _repository.GetAllByServiceTypeAsync(serviceTypeId);

    var result = categories
        .Select(cat => new CategoryWithServicesResponseModel
        {
            Id = cat.Id,
            CategoryName = cat.CategoryName,
            SubCategories = cat.SubCategories
                .Select(sub => new SubCategoryWithServicesResponseModel
                {
                    Id = sub.Id,
                    SubCategoryName = sub.SubCategoryName,
                    Services = ToResponseModel<IEnumerable<GetServicesListResponseModel>>(sub.Services)
                                   .ToList()
                })
                .ToList()
        })
        .ToList();

    return new ServiceTypeFullDataResponseModel { Categories = result };
}

        private async Task SaveFiltersAsync(
            int serviceId,
            List<string>? items,
            ServiceInclusionExclusionType type)
        {
            if (items is null || items.Count == 0) return;

            var filters = items
                .Where(i => !string.IsNullOrWhiteSpace(i))
                    .Select(i => new ServiceInclusionExclusion
                    {
                        ServiceId = serviceId,
                        Item = i.Trim(),
                        Type = type,
                    });

            await AddRangeAsync(_filtersRepository, filters);
        }
    }
}