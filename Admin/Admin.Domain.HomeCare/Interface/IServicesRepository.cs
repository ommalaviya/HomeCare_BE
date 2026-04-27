using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Admin.Domain.HomeCare.Interface
{
    public interface IServicesRepository : IGenericRepository<ServicesOfSubCategory>
    {
        Task<List<Category>> GetAllByServiceTypeAsync(int serviceTypeId);
        
        Task<decimal> GetMaxPriceBySubCategoryAsync(int subCategoryId);
    }
}