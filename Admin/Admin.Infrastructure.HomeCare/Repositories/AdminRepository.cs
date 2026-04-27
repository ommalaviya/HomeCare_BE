using Infrastructure.HomeCare.Data;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;
using Domain.HomeCare.Interfaces;

namespace Infrastructure.HomeCare.Repositories
{
    public class AdminRepository(HomeCareDbContext dbContext) 
        : GenericRepository<AdminUser>(dbContext), IAdminRepository
    {
        
    }
}