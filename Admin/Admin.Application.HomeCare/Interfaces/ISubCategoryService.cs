using Admin.Domain.HomeCare.DataModels.Request.SubCategory;
using Admin.Domain.HomeCare.DataModels.Response.SubCategory;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface ISubCategoryService : IGenericService<SubCategory>
    {
        Task<DataQueryResponseModel<GetSubCategoryResponseModel>>
            GetSubCategoryByCategoryAsync(int categoryId);

        Task<GetSubCategoryResponseModel>
            CreateSubCategoryAsync(CreateSubCategoryRequestModel request);
        Task<bool> SoftDeleteSubCategoryAsync(int id);
              
    }
}