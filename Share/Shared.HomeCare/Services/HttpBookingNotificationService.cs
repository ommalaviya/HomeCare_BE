using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Resources;

namespace Shared.HomeCare.Services
{
    public class HttpBookingNotificationService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<HttpBookingNotificationService> logger)
        : IBookingNotificationService
    {
        private const string ClientName = "AdminApiInternal";

        public async Task NotifyNewBookingAsync(BookingNotifyRequest request)
        {
            try
            {
                var client = httpClientFactory.CreateClient(ClientName);
                var baseUrl = configuration["AdminApi:BaseUrl"]
                    ?? throw new InvalidOperationException(Messages.BaseURLConfiguration);
 
                var secret = configuration["AdminApi:InternalSecret"];
                if (!string.IsNullOrWhiteSpace(secret))
                    client.DefaultRequestHeaders.TryAddWithoutValidation("X-Internal-Secret", secret);
 
                var response = await client.PostAsJsonAsync(
                    $"{baseUrl}/api/internal/booking-notify",
                    request);
 
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning(
                        "Admin booking-notify returned {StatusCode} for BookingId={BookingId}",
                        response.StatusCode, request.BookingId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to notify admin of new booking {BookingId}", request.BookingId);
            }
        }
    }
}