using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Public.Infrastructure.HomeCare.Repositories
{
    public class UserRepository(HomeCareDbContext dbContext)
        : GenericRepository<User>(dbContext), IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email)
            => await dbContext.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower() && !x.IsDeleted);

        public async Task<User?> GetFreshByIdAsync(int id)
            => await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }
}