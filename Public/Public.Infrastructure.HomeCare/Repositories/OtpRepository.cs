using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Public.Infrastructure.HomeCare.Repositories
{
    public class OtpRepository(HomeCareDbContext dbContext)
        : GenericRepository<Otp>(dbContext), IOtpRepository
    {
        public async Task<Otp?> GetByEmailAsync(string email)
        {
            return await dbContext.Otps
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<Otp?> GetValidOtpAsync(string email, string hashedCode)
        {
            return await dbContext.Otps.FirstOrDefaultAsync(x =>
                x.Email.ToLower() == email.ToLower() &&
                x.Code == hashedCode &&
                !x.IsUsed &&
                x.ExpiryAt >= DateTime.UtcNow
            );
        }

        public async Task<Otp?> GetByRefreshTokenAsync(string hashedRefreshToken)
        {
            return await dbContext.Otps.FirstOrDefaultAsync(x =>
                x.RefreshTokenHash == hashedRefreshToken &&
                x.RefreshTokenExpiryAt >= DateTime.UtcNow
            );
        }

        public async Task<Otp?> GetActiveRefreshTokenRowByEmailAsync(string email)
        {
            return await dbContext.Otps
                .Where(x => x.Email.ToLower() == email.ToLower() &&
                            x.RefreshTokenHash != null &&
                            x.RefreshTokenExpiryAt >= DateTime.UtcNow)
                .OrderByDescending(x => x.RefreshTokenExpiryAt)
                .FirstOrDefaultAsync();
        }
    }
}