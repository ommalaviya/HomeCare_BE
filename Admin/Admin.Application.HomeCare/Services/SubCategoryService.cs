using Shared.HomeCare.Entities;
using Shared.HomeCare.Services;
using Shared.HomeCare.Interfaces.Repositories;
using AutoMapper;
using Shared.HomeCare.DataModel.Response;
using System.Linq.Expressions;
using Shared.HomeCare.Resources;
using Admin.Domain.HomeCare.DataModels.Response.SubCategory;
using Admin.Domain.HomeCare.DataModels.Request.SubCategory;
using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.Interface;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class SubCategoryService(
        IGenericRepository<SubCategory> genericRepository,
        ClaimsPrincipal principal,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IGenericRepository<ServicesOfSubCategory> servicesRepository,
        IServicesService servicesService,
        IBookingRepository bookingRepository
       )
        : GenericService<SubCategory>(genericRepository, unitOfWork, mapper, principal),
          ISubCategoryService
    {
        public async Task<DataQueryResponseModel<GetSubCategoryResponseModel>>
            GetSubCategoryByCategoryAsync(int categoryId)
        {
            Expression<Func<SubCategory, bool>> expression =
                x => x.CategoryId == categoryId && !x.IsDeleted;

            var response = await GetAllAsync(expression);
            return new DataQueryResponseModel<GetSubCategoryResponseModel>
            {
                TotalRecords = response.TotalRecords,
                Records = ToResponseModel<IEnumerable<GetSubCategoryResponseModel>>(response.Records),
            };
        }

        public async Task<GetSubCategoryResponseModel>
            CreateSubCategoryAsync(CreateSubCategoryRequestModel request)
        {
            await ThrowIfDuplicateAsync(
                x => x.CategoryId == request.CategoryId &&
                     x.SubCategoryName == request.SubCategoryName &&
                     !x.IsDeleted,
                string.Format(Messages.DuplicateRecord, Messages.SubCategory));

            var result = await AddAsync(ToEntity(request));
            return ToResponseModel<GetSubCategoryResponseModel>(result);
        }

        public async Task<bool> SoftDeleteSubCategoryAsync(int subCategoryId)
        {
            var subCategory = await GetOrThrowAsync(subCategoryId,
                string.Format(Messages.NotFound, Messages.SubCategory));

            if (await bookingRepository.HasActiveBookingsForSubCategoryAsync(subCategoryId))
                throw new InvalidOperationException(string.Format(Messages.CannotDeleteActiveBookings,Messages.SubCategory));

            var servicesResult = await servicesRepository.GetAllAsync(
                predicate: x => x.SubCategoryId == subCategoryId && !x.IsDeleted);

            foreach (var service in servicesResult.Records)
            {
                await servicesService.DeleteServiceAsync(service.Id);
            }

            await SoftDeleteAsync(subCategory);
            return true;
        }
    }
}