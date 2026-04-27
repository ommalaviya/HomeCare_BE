using Shared.HomeCare.DataModel.Request;

namespace Shared.HomeCare.Interfaces.Services
{
    public interface IBookingNotificationService
    {
        Task NotifyNewBookingAsync(BookingNotifyRequest request);
    }
}