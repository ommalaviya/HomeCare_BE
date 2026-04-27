using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;

namespace Admin.Api.HomeCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController(IDashboardService dashboardService) : ControllerBase
    {
        [HttpGet("cards/total-services-booked")]
        public async Task<IActionResult> GetTotalServicesBookedAsync([FromQuery] TotalServicesBookedRequestModel request)
        {
            var result = await dashboardService.GetTotalServicesBookedAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("cards/active-users")]
        public async Task<IActionResult> GetActiveUsersAsync()
        {
            var result = await dashboardService.GetActiveUsersAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("cards/active-service-partners")]
        public async Task<IActionResult> GetActiveServicePartnersAsync()
        {
            var result = await dashboardService.GetActiveServicePartnersAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("cards/total-revenue")]
        public async Task<IActionResult> GetTotalRevenueCardAsync()
        {
            var result = await dashboardService.GetTotalRevenueCardAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("top-performing-services")]
        public async Task<IActionResult> GetTopPerformingServicesAsync([FromQuery] TopPerformingServicesRequestModel request)
        {
            var result = await dashboardService.GetTopPerformingServicesAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("revenue-overview")]
        public async Task<IActionResult> GetRevenueOverviewAsync([FromQuery] RevenueOverviewRequestModel request)
        {
            var result = await dashboardService.GetRevenueOverviewAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("city-bookings-chart")]
        public async Task<IActionResult> GetCityBookingsChartAsync([FromQuery] CityBookingsChartRequestModel request)
        {
            var result = await dashboardService.GetCityBookingsChartAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("top-service-partners")]
        public async Task<IActionResult> GetTopServicePartnersAsync([FromQuery] TopServicePartnersRequestModel request)
        {
            var result = await dashboardService.GetTopServicePartnersAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result));
        }
    }
}