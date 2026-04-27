using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Admin.Domain.HomeCare.Interface
{
    public interface IAdminUserRepository : IGenericRepository<AdminUser>
    {
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<bool> MobileExistsAsync(string mobile, int? excludeId = null);
        Task<AdminUser?> GetDeletedByEmailAsync(string email);
    }
}