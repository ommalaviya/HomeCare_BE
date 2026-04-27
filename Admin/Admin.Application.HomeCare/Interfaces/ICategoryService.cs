using System;
using Admin.Domain.HomeCare.DataModels.Request.Category;
using Admin.Domain.HomeCare.DataModels.Response.Category;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface ICategoryService : IGenericService<Category>
    {
        Task<DataQueryResponseModel<GetCategoryResponseModel>>
            GetCategoryByServiceTypeAsync(int serviceTypeId);

        Task<GetCategoryResponseModel>
            CreateCategoryAsync(CreateCategoryRequestModel request);
            Task<bool> SoftDeleteCategoryAsync(int id);
            
    }
}
