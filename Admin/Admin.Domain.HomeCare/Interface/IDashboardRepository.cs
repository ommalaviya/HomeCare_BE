using Admin.Domain.HomeCare.DataModels.Request.Dashboard;
using Admin.Domain.HomeCare.DataModels.Response.Dashboard;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Admin.Domain.HomeCare.Interface
{
    public interface IDashboardRepository : IGenericRepository<Booking>
    {
        Task<MetricCardModel> GetTotalServicesBookedCardAsync(TotalServicesBookedRequestModel request);
        Task<MetricCardModel> GetActiveUsersCardAsync();
        Task<MetricCardModel> GetActiveServicePartnersCardAsync();
        Task<MetricCardModel> GetTotalRevenueCardAsync();
        Task<List<BookingByServiceTypeResponseModel>> GetBookingsByServiceTypeAsync(TopPerformingServicesRequestModel request);
        Task<List<WeeklyRevenueResponseModel>> GetRevenueAsync(RevenueOverviewRequestModel request);
        Task<List<CityBookingResponseModel>> GetCityBookingsAsync(CityBookingsChartRequestModel request);
        Task<List<TopServicePartnerResponseModel>> GetTopServicePartnersAsync(TopServicePartnersRequestModel request);
    }
}