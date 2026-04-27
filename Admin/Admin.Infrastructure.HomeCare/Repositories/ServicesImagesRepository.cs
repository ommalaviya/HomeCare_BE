using Admin.Domain.HomeCare.Interface;
using Infrastructure.HomeCare.Data;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class ServicesImagesRepository(HomeCareDbContext dbContext)
        : GenericRepository<ServicesImages>(dbContext), IServicesImagesRepository
    {
    }
}