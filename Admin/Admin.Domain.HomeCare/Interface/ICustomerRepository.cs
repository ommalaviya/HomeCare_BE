using Admin.Domain.HomeCare.DataModels.Request.Customer;
using Admin.Domain.HomeCare.DataModels.Response.Customer;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Admin.Domain.HomeCare.Interface
{
    public interface ICustomerRepository : IGenericRepository<User>
    {
        Task<FilteredDataQueryResponse<GetCustomerResponseModel>> GetCustomersWithBookingCountAsync(
            FilterCustomerRequestModel filter);

        Task<CustomerDetailResponse?> GetCustomerDetailAsync(int customerId);

        Task CancelActiveBookingsByUserIdAsync(int userId, string cancellationReason);
    }
}