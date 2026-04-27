using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.BackgroundServices
{
    public class BookingCompletionService(
        IServiceScopeFactory scopeFactory,
        ILogger<BookingCompletionService> logger) : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation(Messages.BookingCompletionServiceStarted);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

                    var completed = await bookingRepo.CompleteExpiredBookingsAsync(stoppingToken);

                    if (completed > 0)
                        logger.LogInformation(
                            string.Format(Messages.BookingCompletionServiceCompleted, completed));
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogError(ex, Messages.BookingCompletionServiceError);
                }

                await Task.Delay(Interval, stoppingToken);
            }

            logger.LogInformation(Messages.BookingCompletionServiceStopped);
        }
    }
}