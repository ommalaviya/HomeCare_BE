using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Public.Domain.HomeCare.Interface
{
    public interface IOtpRepository : IGenericRepository<Otp>
    {
        Task<Otp?> GetByEmailAsync(string email);
        Task<Otp?> GetValidOtpAsync(string email, string hashedCode);
        Task<Otp?> GetByRefreshTokenAsync(string hashedRefreshToken);
        Task<Otp?> GetActiveRefreshTokenRowByEmailAsync(string email);
    }
}