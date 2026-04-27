using Public.Domain.HomeCare.DataModels.Response.ServiceList;
using Shared.HomeCare.DataModel.Response;

namespace Public.Application.HomeCare.Interfaces
{
    public interface IServiceListService
    {
        Task<ServiceTypeWithCategoriesResponseModel> GetCategoriesWithSubCategoriesByServiceTypeIdAsync(int serviceTypeId);

        Task<ServiceListResponseModel> GetServicesBySubCategoryIdAsync(int subCategoryId);
        
        Task<List<ServiceSearchResponseModel>> SearchServicesAsync(int serviceTypeId, string? term);
    }
}
