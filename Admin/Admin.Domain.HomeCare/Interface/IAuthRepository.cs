using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Admin.Domain.HomeCare.Interface
{
    public interface IAuthRepository : IGenericRepository<AdminUser>
    {
        Task<AdminUser?> GetByEmailAsync(string email);

        // Password reset tokens
        Task<AdminPasswordResetToken?> GetValidResetTokenAsync(string token);
        Task AddResetTokenAsync(AdminPasswordResetToken token);
        Task InvalidateAllResetTokensAsync(int adminId);

        // Refresh tokens
        Task AddRefreshTokenAsync(AdminRefreshToken refreshToken);
        Task<AdminRefreshToken?> GetActiveRefreshTokenAsync(string tokenHash);
        Task<AdminRefreshToken?> GetRefreshTokenByHashAsync(string tokenHash); 
        Task UpdateRefreshTokenAsync(AdminRefreshToken refreshToken);
        Task RevokeAllRefreshTokensAsync(int adminId);
    }
}