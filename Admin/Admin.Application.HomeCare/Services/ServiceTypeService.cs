using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.ServiceTypes;
using Admin.Domain.HomeCare.DataModels.Response.ServiceTypes;
using Admin.Domain.HomeCare.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class ServiceTypeService : GenericService<ServiceTypes>, IServiceTypeService
    {
        private const string SubFolder = "ServiceType";

        private readonly IServiceTypeRepository _repo;
        private readonly IFileService _fileService;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ICategoryService _categoryService;
        private readonly IBookingRepository _bookingRepository;

        public ServiceTypeService(
            IServiceTypeRepository repo,
            ICategoryRepository categoryRepo,
            ICategoryService categoryService,
            ClaimsPrincipal principal,
            IFileService fileService,
            IUnitOfWork unitOfWork,
            AutoMapper.IMapper mapper,
            IBookingRepository bookingRepository
            )
            : base(repo, unitOfWork, mapper, principal)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
            _categoryService = categoryService;
            _fileService = fileService;
            _bookingRepository = bookingRepository;
        }

        public async Task<DataQueryResponseModel<GetServiceTypeResponseModel>> GetServiceTypesAsync()
        {
            var response = await GetAllAsync(predicate: x => !x.IsDeleted);
            return new DataQueryResponseModel<GetServiceTypeResponseModel>
            {
                TotalRecords = response.TotalRecords,
                Records = ToResponseModel<IEnumerable<GetServiceTypeResponseModel>>(response.Records),
            };
        }

        public async Task<GetServiceTypeResponseModel> GetServiceTypeByIdAsync(int id)
        {
            var entity = await GetOrThrowAsync(id,
                string.Format(Messages.NotFound, Messages.ServiceType));

            return ToResponseModel<GetServiceTypeResponseModel>(entity);
        }

        public async Task<FileContentHttpResult> GetServiceTypeImageAsync(int id)
        {
            var entity = await GetOrThrowAsync(id,
                string.Format(Messages.NotFound, Messages.ServiceType));

            if (string.IsNullOrWhiteSpace(entity.ImageName))
                throw new FileNotFoundException(
                    string.Format(Messages.NoImage, Messages.ServiceType));

            return await _fileService.GetImageResultAsync(entity.ImageName, SubFolder);
        }

        public async Task<GetServiceTypeResponseModel> CreateServiceTypeAsync(CreateServiceTypeRequestModel request)
        {
            await ThrowIfDuplicateAsync(
                x => x.ServiceName == request.ServiceName && !x.IsDeleted,
                string.Format(Messages.DuplicateRecord, Messages.ServiceType));

            string imageName;

            if (request.Image is not null && request.Image.Length > 0)
            {
                imageName = await _fileService.SaveImageAsync(request.Image, SubFolder);
            }
            else
            {
                const string defaultImageName = "defaultimageservicetype.png";
                imageName = _fileService.GetDefaultImageName(SubFolder)
                    ?? throw new FileNotFoundException(
                        string.Format(Messages.DefaultImageNotFound, SubFolder, defaultImageName));
            }

            var entity = ToEntity(request);
            entity.ImageName = imageName;

            var saved = await AddAsync(entity);
            return ToResponseModel<GetServiceTypeResponseModel>(saved);
        }

        public async Task<GetServiceTypeResponseModel> UpdateServiceTypeAsync(UpdateServiceTypeRequestModel request)
        {
            var entity = await GetOrThrowAsync(request.Id,
                string.Format(Messages.NotFound, Messages.ServiceType));

            await ThrowIfDuplicateAsync(
                x => x.Id != request.Id && x.ServiceName == request.ServiceName && !x.IsDeleted,
                string.Format(Messages.DuplicateRecord, Messages.ServiceType));

            Map<ServiceTypes, UpdateServiceTypeRequestModel>(request, entity);

            if (request.Image is not null && request.Image.Length > 0)
                entity.ImageName = await _fileService.UpdateImageAsync(
                    request.Image, entity.ImageName ?? string.Empty, SubFolder);

            await UpdateAsync(entity);
            return ToResponseModel<GetServiceTypeResponseModel>(entity);
        }

        public async Task<bool> SoftDeleteServiceTypeAsync(int id)
        {
            var entity = await GetOrThrowAsync(id,
                string.Format(Messages.NotFound, Messages.ServiceType));

            if (await _bookingRepository.HasActiveBookingsForServiceTypeAsync(id))
                throw new InvalidOperationException(string.Format(Messages.CannotDeleteActiveBookings,Messages.ServiceType));
            var categoriesResult = await _categoryRepo.GetAllAsync(
                predicate: x => x.ServiceTypeId == id && !x.IsDeleted);

            foreach (var category in categoriesResult.Records)
            {
                await _categoryService.SoftDeleteCategoryAsync(category.Id);
            }

            await SoftDeleteAsync(entity);
            return true;
        }
    }
}