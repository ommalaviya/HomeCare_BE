namespace Admin.Domain.HomeCare.DataModels.Response.Dashboard
{
    public class MetricCardModel
    {
        public decimal CurrentValue { get; set; }
        public decimal PreviousValue { get; set; }
        public double ChangePercent => PreviousValue == 0
            ? (CurrentValue > 0 ? 100.0 : 0.0)
            : Math.Round((double)((CurrentValue - PreviousValue) / PreviousValue) * 100, 1);
        public bool IsIncrease => CurrentValue >= PreviousValue;
    }

    public class BookingByServiceTypeResponseModel
    {
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
    }

    public class WeeklyRevenueResponseModel
    {
        public string DayName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class CityBookingResponseModel
    {
        public string CityName { get; set; } = string.Empty;
        public List<DailyBookingPointResponseModel> Points { get; set; } = [];
    }

    public class DailyBookingPointResponseModel
    {
        public string DayName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
    }

    public class TopServicePartnerResponseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string ServiceTypeName { get; set; } = string.Empty;
        public int TotalJobsCompleted { get; set; }
    }

    public class CityBookingsResponseModel
    {
        public List<CityBookingResponseModel> Cities { get; set; } = [];
    }
}