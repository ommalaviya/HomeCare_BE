using Admin.Domain.HomeCare.DataModels.Request.AdminUser;
using Admin.Domain.HomeCare.DataModels.Response.AdminUser;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface IAdminUserService : IGenericService<AdminUser>
    {
        Task<DataQueryResponseModel<GetAdminUserResponseModel>> GetAdminUsersAsync(FilterAdminUserRequestModel filter);
        Task<GetAdminUserResponseModel> GetAdminUserByIdAsync(int id);
        Task<GetAdminUserResponseModel> CreateAdminUserAsync(CreateAdminUserRequestModel request);
        Task<GetAdminUserResponseModel> UpdateAdminUserAsync(UpdateAdminUserRequestModel request);
        Task<bool> DeleteAdminUserAsync(int id);
        Task ChangeAdminUserPasswordAsync(ChangeAdminUserPasswordRequestModel request);
    }
}