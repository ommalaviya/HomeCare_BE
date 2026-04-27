using Admin.Domain.HomeCare.DataModels.Request.Dashboard;
using Admin.Domain.HomeCare.DataModels.Response.Dashboard;
using Admin.Domain.HomeCare.Interface;
using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Repositories;

namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class DashboardRepository(HomeCareDbContext dbContext)
        : GenericRepository<Booking>(dbContext), IDashboardRepository
    {
        // Card 1
        public async Task<MetricCardModel> GetTotalServicesBookedCardAsync(TotalServicesBookedRequestModel request)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var from = GetFromDate(request.Period, today);

            var duration = today.DayNumber - from.DayNumber;
            var prevFrom = from.AddDays(-duration - 1);
            var prevTo = from.AddDays(-1);

            return new MetricCardModel
            {
                CurrentValue = await dbContext.Bookings.CountAsync(b => b.BookingDate >= from && b.BookingDate <= today && b.Status == BookingStatus.Completed && b.PaymentStatus == PaymentStatus.Success),
                PreviousValue = await dbContext.Bookings.CountAsync(b => b.BookingDate >= prevFrom && b.BookingDate <= prevTo && b.Status == BookingStatus.Completed && b.PaymentStatus == PaymentStatus.Success),
            };
        }

        // Card 2
        public async Task<MetricCardModel> GetActiveUsersCardAsync()
        {
            var (_, _, _, thisMondayDt, prevMondayDt, prevSundayDt, _) = GetWeekBoundaries();

            var current = await dbContext.Users.CountAsync(u => u.Status == "Active" && !u.IsDeleted);
            var thisWeek = await dbContext.Users.CountAsync(u => u.Status == "Active" && !u.IsDeleted && u.CreatedAt >= thisMondayDt);
            var prevWeek = await dbContext.Users.CountAsync(u => u.Status == "Active" && !u.IsDeleted && u.CreatedAt >= prevMondayDt && u.CreatedAt <= prevSundayDt);

            return new MetricCardModel { CurrentValue = current, PreviousValue = Math.Max(0, current - thisWeek + prevWeek) };
        }

        // Card 3
        public async Task<MetricCardModel> GetActiveServicePartnersCardAsync()
        {
            var (_, _, _, thisMondayDt, prevMondayDt, prevSundayDt, _) = GetWeekBoundaries();

            var current = await dbContext.ServicePartners.CountAsync(sp => sp.Status == ServicePartnerStatus.Active && !sp.IsDeleted);
            var thisWeek = await dbContext.ServicePartners.CountAsync(sp => sp.Status == ServicePartnerStatus.Active && !sp.IsDeleted && sp.CreatedAt >= thisMondayDt);
            var prevWeek = await dbContext.ServicePartners.CountAsync(sp => sp.Status == ServicePartnerStatus.Active && !sp.IsDeleted && sp.CreatedAt >= prevMondayDt && sp.CreatedAt <= prevSundayDt);

            return new MetricCardModel { CurrentValue = current, PreviousValue = Math.Max(0, current - thisWeek + prevWeek) };
        }

        // Card 4
        public async Task<MetricCardModel> GetTotalRevenueCardAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var prevYearStart = new DateOnly(today.Year - 1, 1, 1);
            var prevYearEnd = new DateOnly(today.Year - 1, 12, 31);

            return new MetricCardModel
            {
                CurrentValue = (decimal)(await dbContext.Bookings
                    .Where(b => b.Status == BookingStatus.Completed && b.PaymentStatus == PaymentStatus.Success)
                    .SumAsync(b => (decimal?)b.BookingAmount) ?? 0m),
                PreviousValue = (decimal)(await dbContext.Bookings
                    .Where(b => b.BookingDate >= prevYearStart && b.BookingDate <= prevYearEnd && b.Status == BookingStatus.Completed && b.PaymentStatus == PaymentStatus.Success)
                    .SumAsync(b => (decimal?)b.BookingAmount) ?? 0m),
            };
        }

        // Card 5
        public async Task<List<BookingByServiceTypeResponseModel>> GetBookingsByServiceTypeAsync(TopPerformingServicesRequestModel request)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var from = GetFromDate(request.Period, today);

            return await dbContext.Bookings
                .Where(b => b.BookingDate >= from && b.BookingDate <= today && b.Status == BookingStatus.Completed && b.PaymentStatus == PaymentStatus.Success)
                .Include(b => b.ServiceType)
                .Where(b => b.ServiceType != null)
                .GroupBy(b => new { b.ServiceTypeId, b.ServiceType!.ServiceName })
                .Select(g => new BookingByServiceTypeResponseModel
                {
                    ServiceTypeId = g.Key.ServiceTypeId,
                    ServiceTypeName = g.Key.ServiceName ?? string.Empty,
                    BookingCount = g.Count(),
                })
                .OrderByDescending(x => x.BookingCount)
                .ToListAsync();
        }

        // Card 6
        public async Task<List<WeeklyRevenueResponseModel>> GetRevenueAsync(RevenueOverviewRequestModel request)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var period = request.Period.ToLowerInvariant();

            if (period == "year")
            {
                var yearStart = new DateOnly(today.Year, 1, 1);
                var raw = await dbContext.Bookings
                    .Where(b => b.BookingDate >= yearStart && b.BookingDate <= today && b.Status == BookingStatus.Completed && b.PaymentStatus == PaymentStatus.Success)
                    .GroupBy(b => b.BookingDate.Month)
                    .Select(g => new { Month = g.Key, Revenue = g.Sum(b => b.BookingAmount) })
                    .ToListAsync();

                string[] months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                return Enumerable.Range(1, 12)
                    .Select(m => new WeeklyRevenueResponseModel { DayName = months[m - 1], Revenue = raw.FirstOrDefault(r => r.Month == m)?.Revenue ?? 0m })
                    .ToList();
            }

            if (period == "month")
            {
                var monthStart = new DateOnly(today.Year, today.Month, 1);
                var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
                var monthEnd = new DateOnly(today.Year, today.Month, daysInMonth);
                var raw = await dbContext.Bookings
                    .Where(b => b.BookingDate >= monthStart && b.BookingDate <= monthEnd && b.Status == BookingStatus.Completed && b.PaymentStatus == PaymentStatus.Success)
                    .GroupBy(b => b.BookingDate.Day)
                    .Select(g => new { Day = g.Key, Revenue = g.Sum(b => b.BookingAmount) })
                    .ToListAsync();

                return Enumerable.Range(1, daysInMonth)
                    .Select(d => new WeeklyRevenueResponseModel { DayName = d.ToString(), Revenue = raw.FirstOrDefault(r => r.Day == d)?.Revenue ?? 0m })
                    .ToList();
            }

            var monday = GetFromDate("week", today);
            var weekDays = Enumerable.Range(0, 7).Select(i => monday.AddDays(i)).ToList();
            var weekRaw = await dbContext.Bookings
                .Where(b => b.BookingDate >= weekDays[0] && b.BookingDate <= weekDays[6] && b.Status == BookingStatus.Completed && b.PaymentStatus == PaymentStatus.Success)
                .GroupBy(b => b.BookingDate)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(b => b.BookingAmount) })
                .ToListAsync();

            string[] dayNames = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];
            return weekDays
                .Select((d, i) => new WeeklyRevenueResponseModel { DayName = dayNames[i], Revenue = weekRaw.FirstOrDefault(r => r.Date == d)?.Revenue ?? 0m })
                .ToList();
        }

        // Card 8
        public async Task<List<TopServicePartnerResponseModel>> GetTopServicePartnersAsync(TopServicePartnersRequestModel request)
        {
            var top = Math.Clamp(request.Top, 1, 100);

            var partners = await (
                from sp in dbContext.ServicePartners
                let jobsCompleted = dbContext.Bookings.Count(b => b.AssignedPartnerId == sp.Id && b.Status == BookingStatus.Completed)
                orderby jobsCompleted descending
                select new TopServicePartnerResponseModel
                {
                    Id = sp.Id,
                    FullName = sp.FullName ?? string.Empty,
                    ProfileImageUrl = sp.ProfileImageUrl,
                    ServiceTypeName = sp.ServiceTypes != null ? sp.ServiceTypes.ServiceName ?? string.Empty : string.Empty,
                    TotalJobsCompleted = jobsCompleted,
                })
                .Take(top)
                .ToListAsync();

            return partners;
        }

        // Card 7
        public async Task<List<CityBookingResponseModel>> GetCityBookingsAsync(CityBookingsChartRequestModel request)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var period = request.Period.ToLowerInvariant();
            var from = GetFromDate(period, today);

            string[] labels = period switch
            {
                "year" => ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                "month" => Enumerable.Range(0, 30).Select(i => from.AddDays(i).ToString("dd MMM")).ToArray(),
                _ => ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
            };

            var bookings = await dbContext.Bookings
                .Include(b => b.Address)
                .Where(b => b.BookingDate >= from && b.BookingDate <= today)
                .Select(b => new { b.BookingDate, FullAddress = b.Address != null ? b.Address.FullAddress : null, SaveAs = b.Address != null ? b.Address.SaveAs : null })
                .ToListAsync();

            var cityGroups = bookings
                .Select(b => new { b.BookingDate, City = ExtractCity(b.FullAddress, b.SaveAs) })
                .Where(b => !string.IsNullOrWhiteSpace(b.City))
                .GroupBy(b => b.City, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(g => g.Count())
                .Take(2)
                .ToList();

            return cityGroups.Select(city =>
            {
                var points = period == "year"
                    ? Enumerable.Range(1, 12).Select((m, i) => new DailyBookingPointResponseModel { DayName = labels[i], BookingCount = city.Count(b => b.BookingDate.Month == m) }).ToList()
                    : Enumerable.Range(0, labels.Length).Select(i => new DailyBookingPointResponseModel { DayName = labels[i], BookingCount = city.Count(b => b.BookingDate == from.AddDays(i)) }).ToList();

                return new CityBookingResponseModel { CityName = city.Key, Points = points };
            }).ToList();
        }

        private static (DateOnly thisMonday, DateOnly prevMonday, DateOnly prevSunday,
                         DateTime thisMondayDt, DateTime prevMondayDt, DateTime prevSundayDt,
                         DateOnly today) GetWeekBoundaries()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var thisMonday = today.AddDays(-((int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1));
            var prevMonday = thisMonday.AddDays(-7);
            var prevSunday = thisMonday.AddDays(-1);

            return (thisMonday, prevMonday, prevSunday,
                    thisMonday.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                    prevMonday.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                    prevSunday.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc),
                    today);
        }

        private static DateOnly GetFromDate(string period, DateOnly today) =>
            period.ToLowerInvariant() switch
            {
                "week" => today.AddDays(-((int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1)),
                "month" => new DateOnly(today.Year, today.Month, 1),
                "year" => new DateOnly(today.Year, 1, 1),
                _ => today.AddDays(-7)
            };

        private static string ExtractCity(string? fullAddress, string? saveAs)
        {
            if (!string.IsNullOrWhiteSpace(fullAddress))
            {
                var parts = fullAddress.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 3)
                    return parts[parts.Length - 2];

                return parts[0];
            }

            return saveAs ?? string.Empty;
        }
    }
}