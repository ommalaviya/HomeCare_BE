using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Public.Infrastructure.HomeCare.Repositories
{
    public class AddressRepository(HomeCareDbContext dbContext)
        : GenericRepository<UserAddress>(dbContext), IAddressRepository
    {
        public async Task<IEnumerable<UserAddress>> GetByUserIdAsync(int userId)
        {
            return await dbContext.UserAddresses
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}