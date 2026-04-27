using Public.Domain.HomeCare.DataModels.Request.Booking;
using Public.Domain.HomeCare.DataModels.Request.Payment;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Interfaces.Repositories;

namespace Public.Domain.HomeCare.Interface
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<Booking?> GetByIdWithDetailsAsync(int bookingId);

        Task<ServicePartner?> GetAvailablePartnerAsync(SlotAvailabilityRequestModel request);

        Task<bool> IsPartnerAvailableAsync(SlotAvailabilityRequestModel request);

        Task<bool> HasUserBookedSameSlotAsync(int currentUserId, CreateTransactionIntentRequest request);

        Task<bool> HasUserBookedSameSlotAsync(int currentUserId, CreateBookingRequestModel request);

        Task<bool> HasUserUsedCouponAsync(int userId, int offerId);

        Task<int> CompleteExpiredBookingsAsync(CancellationToken cancellationToken = default);

        Task<List<Booking>> GetMyBookingsAsync(int userId, BookingTab tab);

        Task<List<Booking>> GetMyBookingsByUserAsync(int userId);
    }
}