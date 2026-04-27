using Admin.Domain.HomeCare.Interface;
using Infrastructure.HomeCare.Data;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class ServiceInclusionExclusionRepository(HomeCareDbContext dbContext)
        : GenericRepository<ServiceInclusionExclusion>(dbContext), IServiceInclusionExclusionRepository
    {
    }
}