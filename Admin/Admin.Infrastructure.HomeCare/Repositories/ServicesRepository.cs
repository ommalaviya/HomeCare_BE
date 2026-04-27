using Admin.Domain.HomeCare.Interface;
using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class ServicesRepository(HomeCareDbContext dbContext)
        : GenericRepository<ServicesOfSubCategory>(dbContext), IServicesRepository
    {
        public Task<List<Category>> GetAllByServiceTypeAsync(int serviceTypeId)
    => dbContext.Set<Category>()
        .Include(c => c.SubCategories.Where(sc => !sc.IsDeleted))
            .ThenInclude(sc => sc.Services.Where(s => !s.IsDeleted))
        .Where(c =>
            c.ServiceTypeId == serviceTypeId &&
            !c.IsDeleted)
        .ToListAsync();

        public async Task<decimal> GetMaxPriceBySubCategoryAsync(int subCategoryId)
        {
            var hasAny = await dbContext.Set<ServicesOfSubCategory>()
                .AnyAsync(s => s.SubCategoryId == subCategoryId && !s.IsDeleted);

            if (!hasAny) return 0;

            return await dbContext.Set<ServicesOfSubCategory>()
                .Where(s => s.SubCategoryId == subCategoryId && !s.IsDeleted)
                .MaxAsync(s => s.Price);
        }
    }
}