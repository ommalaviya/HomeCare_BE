using Admin.Domain.HomeCare.DataModels.Request.Dashboard;
using Admin.Domain.HomeCare.DataModels.Response.Dashboard;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface IDashboardService
    {
        Task<MetricCardModel> GetTotalServicesBookedAsync(TotalServicesBookedRequestModel request);
        Task<MetricCardModel> GetActiveUsersAsync();
        Task<MetricCardModel> GetActiveServicePartnersAsync();
        Task<MetricCardModel> GetTotalRevenueCardAsync();
        Task<List<BookingByServiceTypeResponseModel>> GetTopPerformingServicesAsync(TopPerformingServicesRequestModel request);
        Task<List<WeeklyRevenueResponseModel>> GetRevenueOverviewAsync(RevenueOverviewRequestModel request);
        Task<CityBookingsResponseModel> GetCityBookingsChartAsync(CityBookingsChartRequestModel request);
        Task<List<TopServicePartnerResponseModel>> GetTopServicePartnersAsync(TopServicePartnersRequestModel request);
    }
}