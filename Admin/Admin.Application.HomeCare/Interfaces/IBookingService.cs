using Admin.Domain.HomeCare.DataModels.Request.Booking;
using Admin.Domain.HomeCare.DataModels.Response.Booking;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface IBookingService : IGenericService<Booking>
    {
        // Parent grid
        Task<FilteredDataQueryResponseModel<CustomerBookingSummaryResponse>> GetCustomerBookingSummariesAsync(
            FilterBookingRequestModel filter);

        // Child grid (Booking Management expand row)
        Task<IEnumerable<BookingDetailResponse>> GetBookingDetailsByUserIdAsync(
            int userId, FilterBookingRequestModel filter);


        // Available Expert
        Task<IEnumerable<AvailableExpertResponse>> GetAvailableExpertsAsync(
            int serviceTypeId, int? excludeBookingId = null);

        // Change expert
        Task<bool> ChangeExpertAsync(ChangeExpertRequestModel request);

        // Status changes
        Task<bool> CompleteBookingAsync(int bookingId);
        Task<bool> CancelBookingAsync(CancelBookingRequestModel request);

        // Delete
        Task<bool> DeleteBookingsByPaymentAsync(int userId, PaymentMethod paymentMethod);
        Task<bool> DeleteBookingAsync(int bookingId);

        Task<FilteredDataQueryResponseModel<CustomerBookingDetailResponse>> GetCustomerBookingsAsync(
            int customerId, FilterCustomerBookingsRequestModel filter);
    }
}