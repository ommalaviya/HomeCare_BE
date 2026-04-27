using Admin.Domain.HomeCare.DataModels.Request.Customer;
using Admin.Domain.HomeCare.DataModels.Response.Customer;
using Shared.HomeCare.DataModel.Response;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface ICustomerService
    {
        Task<FilteredDataQueryResponseModel<GetCustomerResponseModel>> GetAllCustomersAsync(
            FilterCustomerRequestModel filter);

        Task<GetCustomerResponseModel> CreateCustomerAsync(CreateCustomerRequestModel request);

        Task<bool> UpdateStatusAsync(int id);

        Task<bool> SoftDeleteCustomerAsync(int id);
        Task<CustomerDetailResponse> GetCustomerDetailAsync(int customerId);
    }
}