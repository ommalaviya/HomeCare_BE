using Public.Domain.HomeCare.DataModels.Request.Booking;
using Public.Domain.HomeCare.DataModels.Response.Booking;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Public.Application.HomeCare.Interfaces
{
    public interface IBookingService : IGenericService<Booking>
    {
        Task<BookingResponseModel> CreateBookingAsync(CreateBookingRequestModel request);

        Task<List<BookingResponseModel>> GetMyBookingsAsync();

        Task<SlotAvailabilityResponseModel> CheckSlotAvailabilityAsync(
            SlotAvailabilityRequestModel request);

        Task<List<MyBookingResponseModel>> GetMyBookingsByTabAsync(MyBookingsRequestModel request);

        Task<BookingResponseModel> GetBookingByIdAsync(int bookingId);
    }
}