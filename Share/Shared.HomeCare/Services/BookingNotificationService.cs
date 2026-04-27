using Microsoft.AspNetCore.SignalR;
using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.Hubs;
using Shared.HomeCare.Interfaces.Services;

namespace Shared.HomeCare.Services
{
    public class BookingNotificationService(IHubContext<BookingHub> hubContext)
        : IBookingNotificationService
    {
        public Task NotifyNewBookingAsync(BookingNotifyRequest request)
            => hubContext.Clients.All.SendAsync("NewBookingCreated", request);
    }
}