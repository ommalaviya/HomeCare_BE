using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Public.Domain.HomeCare.DataModels.Request.Booking;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Repositories;
using Public.Domain.HomeCare.DataModels.Request.Payment;

namespace Public.Infrastructure.HomeCare.Repositories
{
    public class BookingRepository(HomeCareDbContext dbContext)
        : GenericRepository<Booking>(dbContext), IBookingRepository
    {
        public async Task<Booking?> GetByIdWithDetailsAsync(int bookingId)
        {
            return await dbContext.Bookings
                .Include(b => b.Address)
                .Include(b => b.Service)
                .Include(b => b.ServiceType)
                .Include(b => b.AssignedPartner)
                .Include(b => b.Offer)
                .Include(b => b.CouponUsage)
                     .ThenInclude(cu => cu.Coupon)
                .FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted);
        }

        public async Task<bool> HasUserBookedSameSlotAsync(int currentUserId, CreateBookingRequestModel request)
        {
            return await dbContext.Bookings.AnyAsync(b =>
                !b.IsDeleted &&
                b.UserId == currentUserId &&
                b.ServiceId == request.ServiceId &&
                b.BookingDate == request.BookingDate &&
                b.BookingTime == request.BookingTime &&
                b.Status != BookingStatus.Cancelled &&
                !(b.Status == BookingStatus.Pending && b.PaymentStatus == PaymentStatus.Failed));
        }

        public async Task<bool> HasUserBookedSameSlotAsync(int currentUserId, CreateTransactionIntentRequest request)
        {
            return await dbContext.Bookings.AnyAsync(b =>
                !b.IsDeleted &&
                b.UserId == currentUserId &&
                b.ServiceId == request.ServiceId &&
                b.BookingDate == request.BookingDate &&
                b.BookingTime == request.BookingTime &&
                b.Status != BookingStatus.Cancelled &&
                !(b.Status == BookingStatus.Pending && b.PaymentStatus == PaymentStatus.Failed));
        }

        public async Task<ServicePartner?> GetAvailablePartnerAsync(SlotAvailabilityRequestModel request)
        {
            if (!TimeSpan.TryParse(request.BookingTime, out var requestedStart))
                return null;

            int requestedDuration;
            if (request.KnownDurationMinutes.HasValue && request.KnownDurationMinutes.Value > 0)
            {
                requestedDuration = request.KnownDurationMinutes.Value;
            }
            else
            {
                var durationStr = await dbContext.Services
                    .Where(s => s.Id == request.ServiceId)
                    .Select(s => s.Duration)
                    .FirstOrDefaultAsync();

                if (!int.TryParse(durationStr, out requestedDuration) || requestedDuration <= 0)
                    return null;
            }

            var requestedEnd = requestedStart.Add(TimeSpan.FromMinutes(requestedDuration));

            var bookingsOnDate = await dbContext.Bookings
                .Where(b =>
                    b.BookingDate == request.BookingDate &&
                    b.Status != BookingStatus.Cancelled &&
                    b.AssignedPartnerId != null)
                .Select(b => new
                {
                    PartnerId = b.AssignedPartnerId!.Value,
                    b.BookingTime,
                    b.DurationInMinutes
                })
                .ToListAsync();

            var busyPartnerIds = bookingsOnDate
                .Where(b =>
                {
                    if (!TimeSpan.TryParse(b.BookingTime, out var existingStart))
                        return false;

                    var effectiveDuration = b.DurationInMinutes > 0 ? b.DurationInMinutes : requestedDuration;
                    var existingEnd = existingStart.Add(TimeSpan.FromMinutes(effectiveDuration));
                    return requestedStart < existingEnd && existingStart < requestedEnd;
                })
                .Select(b => b.PartnerId)
                .ToHashSet();

            return await dbContext.ServicePartners
                .Where(sp =>
                    sp.ApplyingForTypeId == request.ServiceTypeId &&
                   sp.Status == ServicePartnerStatus.Active &&
                   sp.VerificationStatus == VerificationStatus.Verified &&
                   !sp.IsDeleted &&
                    !busyPartnerIds.Contains(sp.Id))
                .OrderBy(sp => sp.TotalJobsCompleted)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsPartnerAvailableAsync(SlotAvailabilityRequestModel request)
        {
            var partner = await GetAvailablePartnerAsync(request);
            return partner is not null;
        }

        public async Task<bool> HasUserUsedCouponAsync(int userId, int offerId)
        {
            return await dbContext.CouponUsages
                .AnyAsync(cu => cu.UserId == userId && cu.CouponId == offerId);
        }

        public async Task<List<Booking>> GetMyBookingsByUserAsync(int userId)
        {
            return await dbContext.Bookings
                .Include(b => b.Address)
                .Include(b => b.Service)
                .Include(b => b.ServiceType)
                .Include(b => b.AssignedPartner)
                .Where(b => b.UserId == userId && !b.IsDeleted)
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.BookingTime)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetMyBookingsAsync(int userId, BookingTab tab)
        {
            var upcomingStatuses = new[] { BookingStatus.Pending, BookingStatus.Confirmed };
            var completedStatuses = new[] { BookingStatus.Completed, BookingStatus.Cancelled };

            var statuses = tab == BookingTab.Upcoming ? upcomingStatuses : completedStatuses;

            return await dbContext.Bookings
                .Include(b => b.Service)
                .Include(b => b.ServiceType)
                .Include(b => b.Address)
                .Include(b => b.AssignedPartner)
                .Where(b =>
                    b.UserId == userId &&
                    !b.IsDeleted &&
                    statuses.Contains(b.Status))
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.BookingTime)
                .ToListAsync();
        }

        public async Task<int> CompleteExpiredBookingsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);

            var candidates = await dbContext.Bookings
                .Where(b =>
                    b.Status == BookingStatus.Pending &&
                    b.BookingDate <= today)
                .Select(b => new
                {
                    b.Id,
                    b.BookingDate,
                    b.BookingTime,
                    b.DurationInMinutes,
                    b.PaymentMethod,
                    b.PaymentStatus
                })
                .ToListAsync(cancellationToken);

            if (candidates.Count == 0)
                return 0;

            var expiredIds = new List<int>(candidates.Count);
            var cashPendingIds = new List<int>();

            foreach (var b in candidates)
            {
                if (!TimeSpan.TryParse(b.BookingTime, out var startSpan))
                    continue;

                var serviceEnd = b.BookingDate
                    .ToDateTime(TimeOnly.MinValue)
                    .Add(startSpan)
                    .AddMinutes(b.DurationInMinutes);

                if (now < serviceEnd)
                    continue;

                expiredIds.Add(b.Id);

                if (b.PaymentMethod == PaymentMethod.Cash &&
                    b.PaymentStatus == PaymentStatus.Pending)
                    cashPendingIds.Add(b.Id);
            }

            if (expiredIds.Count == 0)
                return 0;

            await dbContext.Bookings
                .Where(b => expiredIds.Contains(b.Id))
                .ExecuteUpdateAsync(
                    s => s.SetProperty(b => b.Status, BookingStatus.Completed),
                    cancellationToken);

            if (cashPendingIds.Count > 0)
            {
                await dbContext.Bookings
                    .Where(b => cashPendingIds.Contains(b.Id))
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(b => b.PaymentStatus, PaymentStatus.Success),
                        cancellationToken);

                await dbContext.Transactions
                    .Where(t => cashPendingIds.Contains(t.BookingId))
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(t => t.PaymentStatus, PaymentStatus.Success),
                        cancellationToken);
            }

            return expiredIds.Count;
        }
    }
}