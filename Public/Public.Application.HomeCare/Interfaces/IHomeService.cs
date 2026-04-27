using Public.Domain.HomeCare.DataModels.Response.Home;

namespace Public.Application.HomeCare.Interfaces{
    public interface IHomeService
    {
        Task<List<ServiceNamesResponseModel>> GetServiceNamesAsync();
        Task<List<ServiceTypeResponseModel>> GetServiceTypesAsync();
        Task<List<ServiceResponseModel>> GetPopularServicesAsync();
        Task<List<ServiceResponseModel>> GetAllServicesAsync();
        Task<CountResponseModel> GetCountsAsync();
    }
}