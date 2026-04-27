using Public.Domain.HomeCare.DataModels.Response.Users;

namespace Public.Application.HomeCare.Interfaces
{
    public interface IOtpService
    {
        Task SendOtpAsync(string email);
        Task<AuthResponseModel?> VerifyOtpAsync(string email, string code, string? name);
        Task<AuthResponseModel?> RefreshTokenAsync();
        void Logout();
    }
}