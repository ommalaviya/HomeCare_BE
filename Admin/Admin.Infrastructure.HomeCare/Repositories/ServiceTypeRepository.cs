using Infrastructure.HomeCare.Data;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;
using Admin.Domain.HomeCare.Interface;
namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class ServiceTypeRepository(HomeCareDbContext dbContext) : GenericRepository<ServiceTypes>(dbContext), IServiceTypeRepository
    {

    }
}