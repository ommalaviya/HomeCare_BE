using Admin.Domain.HomeCare.Interface;
using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class AdminUserRepository(HomeCareDbContext dbContext)
        : GenericRepository<AdminUser>(dbContext), IAdminUserRepository
    {
        public Task<bool> EmailExistsAsync(string email, int? excludeId = null)
            => dbContext.Set<AdminUser>().AnyAsync(x =>
                x.Email == email &&
                !x.IsDeleted &&
                (excludeId == null || x.Id != excludeId));

        public Task<bool> MobileExistsAsync(string mobile, int? excludeId = null)
            => dbContext.Set<AdminUser>().AnyAsync(x =>
                x.MobileNumber == mobile &&
                !x.IsDeleted &&
                (excludeId == null || x.Id != excludeId));

        public Task<AdminUser?> GetDeletedByEmailAsync(string email)
            => dbContext.Set<AdminUser>().FirstOrDefaultAsync(x => x.Email == email && x.IsDeleted);
    }
}