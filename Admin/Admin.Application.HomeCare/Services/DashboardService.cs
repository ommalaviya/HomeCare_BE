using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Dashboard;
using Admin.Domain.HomeCare.DataModels.Response.Dashboard;
using Admin.Domain.HomeCare.Interface;
using AutoMapper;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Services;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class DashboardService : GenericService<Booking>, IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(
            IDashboardRepository dashboardRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ClaimsPrincipal principal)
            : base(dashboardRepository, unitOfWork, mapper, principal)
        {
            _dashboardRepository = dashboardRepository;
        }

        // Card 1 – Total Services Booked (yearly)
        public async Task<MetricCardModel> GetTotalServicesBookedAsync(TotalServicesBookedRequestModel request)
            => await _dashboardRepository.GetTotalServicesBookedCardAsync(request);

        // Card 2 – Active Users
        public async Task<MetricCardModel> GetActiveUsersAsync()
            => await _dashboardRepository.GetActiveUsersCardAsync();

        // Card 3 – Active Service Partners
        public async Task<MetricCardModel> GetActiveServicePartnersAsync()
            => await _dashboardRepository.GetActiveServicePartnersCardAsync();

        // Card 4 – Total Revenue
        public async Task<MetricCardModel> GetTotalRevenueCardAsync()
            => await _dashboardRepository.GetTotalRevenueCardAsync();

        // Card 5 – Top Performing Services
        public async Task<List<BookingByServiceTypeResponseModel>> GetTopPerformingServicesAsync(TopPerformingServicesRequestModel request)
            => await _dashboardRepository.GetBookingsByServiceTypeAsync(request);

        // Card 6 – Revenue Overview
        public async Task<List<WeeklyRevenueResponseModel>> GetRevenueOverviewAsync(RevenueOverviewRequestModel request)
            => await _dashboardRepository.GetRevenueAsync(request);

        // Card 7 – City Bookings
        public async Task<CityBookingsResponseModel> GetCityBookingsChartAsync(CityBookingsChartRequestModel request)
        {
            var cities = await _dashboardRepository.GetCityBookingsAsync(request);
            return new CityBookingsResponseModel { Cities = cities };
        }

        // Card 8 – Top Service Partners
        public async Task<List<TopServicePartnerResponseModel>> GetTopServicePartnersAsync(TopServicePartnersRequestModel request)
            => await _dashboardRepository.GetTopServicePartnersAsync(request);
    }
}