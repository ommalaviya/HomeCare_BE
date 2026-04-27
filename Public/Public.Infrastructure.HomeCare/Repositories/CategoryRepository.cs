using Infrastructure.HomeCare.Data;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Public.Infrastructure.HomeCare.Repositories
{
    public class CategoryRepository(HomeCareDbContext dbContext)
        : GenericRepository<Category>(dbContext), ICategoryRepository
    {
    }
}
