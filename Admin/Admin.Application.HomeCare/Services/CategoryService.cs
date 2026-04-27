using Shared.HomeCare.Entities;
using Shared.HomeCare.Services;
using Shared.HomeCare.Interfaces.Repositories;
using AutoMapper;
using Shared.HomeCare.DataModel.Response;
using System.Linq.Expressions;
using Shared.HomeCare.Resources;
using Admin.Domain.HomeCare.DataModels.Response.Category;
using Admin.Domain.HomeCare.DataModels.Request.Category;
using Admin.Domain.HomeCare.Interface;
using Admin.Application.HomeCare.Interfaces;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class CategoryService(
        IGenericRepository<Category> genericRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal,
        ICategoryRepository categoryRepository,
        ISubCategoryRepository subCategoryRepository,
        ISubCategoryService subCategoryService,
        IBookingRepository bookingRepository
    )
        : GenericService<Category>(genericRepository, unitOfWork, mapper, principal),
          ICategoryService
    {
        public async Task<DataQueryResponseModel<GetCategoryResponseModel>> GetCategoryByServiceTypeAsync(int serviceTypeId)
        {
            Expression<Func<Category, bool>> expression =
                x => x.ServiceTypeId == serviceTypeId && !x.IsDeleted;

            var includer = new ExpressionIncluder<Category>
            {
                Includes = [x => x.ServiceTypes]
            };

            var response = await GetAllAsync(expression, null, includer);

            return new DataQueryResponseModel<GetCategoryResponseModel>
            {
                TotalRecords = response.TotalRecords,
                Records = ToResponseModel<IEnumerable<GetCategoryResponseModel>>(response.Records),
            };
        }

        public async Task<GetCategoryResponseModel> CreateCategoryAsync(CreateCategoryRequestModel request)
        {
            await ThrowIfDuplicateAsync(
                x => x.ServiceTypeId == request.ServiceTypeId &&
                     x.CategoryName == request.CategoryName &&
                     !x.IsDeleted,
                string.Format(Messages.DuplicateRecord, Messages.Category));

            var result = await AddAsync(ToEntity(request));
            return ToResponseModel<GetCategoryResponseModel>(result);
        }

        public async Task<bool> SoftDeleteCategoryAsync(int id)
        {
            var entity = await GetOrThrowAsync(id,
                string.Format(Messages.NotFound, Messages.Category));

            if (await bookingRepository.HasActiveBookingsForCategoryAsync(id))
                throw new InvalidOperationException(string.Format(Messages.CannotDeleteActiveBookings,Messages.Category));

            var subCategoriesResult = await subCategoryRepository.GetAllAsync(
                predicate: x => x.CategoryId == id && !x.IsDeleted);

            foreach (var subCategory in subCategoriesResult.Records)
            {
                await subCategoryService.SoftDeleteSubCategoryAsync(subCategory.Id);
            }

            await SoftDeleteAsync(entity);
            return true;
        }
    }
}