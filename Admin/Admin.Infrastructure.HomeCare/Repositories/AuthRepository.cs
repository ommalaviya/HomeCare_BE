using Admin.Domain.HomeCare.Interface;
using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class AuthRepository(HomeCareDbContext context)
        : GenericRepository<AdminUser>(context), IAuthRepository
    {
        public Task<AdminUser?> GetByEmailAsync(string email)
            => context.Set<AdminUser>().FirstOrDefaultAsync(a => a.Email == email && !a.IsDeleted);

        public Task<AdminPasswordResetToken?> GetValidResetTokenAsync(string token)
            => context.AdminPasswordResetTokens
                .Include(t => t.Admin)
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);

        public async Task AddResetTokenAsync(AdminPasswordResetToken token)
            => await context.AdminPasswordResetTokens.AddAsync(token);

        public async Task InvalidateAllResetTokensAsync(int adminId)
        {
            var tokens = await context.AdminPasswordResetTokens
                .Where(t => t.AdminId == adminId && !t.IsUsed)
                .ToListAsync();
            foreach (var t in tokens) t.IsUsed = true;
        }

        public async Task AddRefreshTokenAsync(AdminRefreshToken refreshToken)
            => await context.AdminRefreshTokens.AddAsync(refreshToken);

        public Task<AdminRefreshToken?> GetActiveRefreshTokenAsync(string tokenHash)
            => context.AdminRefreshTokens
                .Include(t => t.Admin)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

        //fetches any token by hash regardless of revoked/expired state
        public Task<AdminRefreshToken?> GetRefreshTokenByHashAsync(string tokenHash)
            => context.AdminRefreshTokens
                .Include(t => t.Admin)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

        public Task UpdateRefreshTokenAsync(AdminRefreshToken refreshToken)
        {
            context.AdminRefreshTokens.Update(refreshToken);
            return Task.CompletedTask;
        }

        public async Task RevokeAllRefreshTokensAsync(int adminId)
        {
            var tokens = await context.AdminRefreshTokens
                .Where(t => t.AdminId == adminId && !t.IsRevoked)
                .ToListAsync();
            foreach (var t in tokens) t.IsRevoked = true;
        }

        public new Task<AdminUser?> GetByIdAsync(int id)
            => context.Set<AdminUser>().FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }
}