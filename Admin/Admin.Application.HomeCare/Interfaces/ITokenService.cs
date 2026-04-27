using Admin.Domain.HomeCare.DataModels.Response.Auth;
using Shared.HomeCare.Entities;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface ITokenService
    {
        Task<IssueTokensResultModel> IssueTokensAsync(AdminUser admin);
        Task<RotateTokensResultModel> RotateRefreshTokenAsync(string rawRefreshToken);
        Task RevokeAllTokensAsync(int adminId);
        string GenerateAccessToken(AdminUser admin);
    }
}