using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Shared.Helpers;
using Shared.HomeCare.Interfaces.Services;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class HomeController(IHomeService homeService,IFileService fileService) : ControllerBase
    {
        [HttpGet("services-names")]
        public async Task<IActionResult> GetServiceNamesAsync()
        {
            var result = await homeService.GetServiceNamesAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("service-types")]
        public async Task<IActionResult> GetServiceTypesAsync()
        {
            var result = await homeService.GetServiceTypesAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("popular-services")]
        public async Task<IActionResult> GetPopularServicesAsync()
        {
            var result = await homeService.GetPopularServicesAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("all-services")]
        public async Task<IActionResult> GetAllServicesAsync()
        {
            var result = await homeService.GetAllServicesAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("dashboard-counts")]
        public async Task<IActionResult> GetCountsAsync()
        {
            var result = await homeService.GetCountsAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }
    }
}