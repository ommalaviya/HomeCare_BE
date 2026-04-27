using Infrastructure.HomeCare.Data;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Public.Infrastructure.HomeCare.Repositories
{
    public class ServicePartnerSkillRepository(HomeCareDbContext dbContext)
        : GenericRepository<ServicePartnerSkill>(dbContext), IServicePartnerSkillRepository
    {
    }
}
