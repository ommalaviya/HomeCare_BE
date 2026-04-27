using Infrastructure.HomeCare.Data;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Public.Infrastructure.HomeCare.Repositories
{
    public class ServicePartnerEducationRepository(HomeCareDbContext dbContext)
        : GenericRepository<ServicePartnerEducation>(dbContext), IServicePartnerEducationRepository
    {
    }
}
