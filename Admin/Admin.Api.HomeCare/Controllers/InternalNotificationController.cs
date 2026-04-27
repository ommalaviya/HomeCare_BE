using Shared.HomeCare.DataModel.Request;
using Microsoft.AspNetCore.Mvc;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Resources;

namespace Admin.Api.HomeCare.Controllers
{
    [Route("api/internal")]
    [ApiController]
    public class InternalNotificationController(
        IBookingNotificationService notificationService,
        IConfiguration configuration)
        : ControllerBase
    {
        [HttpPost("booking-notify")]
        public async Task<IActionResult> NotifyNewBooking([FromBody] BookingNotifyRequest request)
        {
            var expectedSecret = configuration["AdminApi:InternalSecret"];
            if (!string.IsNullOrWhiteSpace(expectedSecret))
            {
                if (!Request.Headers.TryGetValue("X-Internal-Secret", out var incoming)
                    || incoming != expectedSecret)
                    return Unauthorized(Messages.Unauthorized);
            }
 
            await notificationService.NotifyNewBookingAsync(request);
 
            return Ok();
        }
    }
}