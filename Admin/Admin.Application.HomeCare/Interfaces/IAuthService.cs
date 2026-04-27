using Admin.Domain.HomeCare.DataModels.Request.Auth;
using Admin.Domain.HomeCare.DataModels.Response.Auth;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface IAuthService : IGenericService<AdminUser>
    {
        Task<LoginResponseModel> LoginAsync(LoginRequestModel model);
        Task<LoginResponseModel> RefreshTokenAsync();
        Task RevokeSessionAsync(int adminId);
        Task<string> ForgotPasswordAsync(ForgotPasswordRequestModel model);
        Task<string> ResetPasswordAsync(ResetPasswordRequestModel model);
        Task ValidateResetTokenAsync(string token);
    }
}