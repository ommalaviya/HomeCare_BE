using Admin.Domain.HomeCare.DataModels.Request.Booking;
using Admin.Domain.HomeCare.DataModels.Response.Booking;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Interfaces.Repositories;

namespace Admin.Domain.HomeCare.Interface
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        /// Parent grid 
        Task<FilteredDataQueryResponse<CustomerBookingSummaryResponse>> GetCustomerBookingSummariesAsync(
            FilterBookingRequestModel filter);

        /// Child grid
        Task<IEnumerable<BookingDetailResponse>> GetBookingDetailsByUserIdAsync(
                    int userId, FilterBookingRequestModel filter);

        Task<IEnumerable<AvailableExpertResponse>> GetAvailableExpertsAsync(
            int serviceTypeId, int? excludeBookingId = null);

        Task<User?> GetUserByIdAsync(int userId);

        Task<IEnumerable<Booking>> GetBookingsByUserAndPaymentAsync(int userId, PaymentMethod paymentMethod);

        Task<int?> GetAssignedPartnerIdAsync(int bookingId);

        Task<FilteredDataQueryResponse<CustomerBookingDetailResponse>> GetCustomerBookingsAsync(
            int customerId, FilterCustomerBookingsRequestModel filter);

        Task IncrementPartnerTotalJobsCompletedAsync(int partnerId);

        Task<bool> HasActiveBookingsForServiceAsync(int serviceId);
        Task<bool> HasActiveBookingsForSubCategoryAsync(int subCategoryId);
        Task<bool> HasActiveBookingsForCategoryAsync(int categoryId);
        Task<bool> HasActiveBookingsForServiceTypeAsync(int serviceTypeId);
    }
}