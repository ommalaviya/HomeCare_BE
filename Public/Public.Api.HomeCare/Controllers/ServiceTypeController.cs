using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Shared.Helpers;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/service-type")]
    [ApiController]
    public class ServiceTypeController(IServiceTypeService serviceTypeService) : ControllerBase
    {
        [HttpGet("with-booking-count")]
        public async Task<IActionResult> GetServiceTypesWithBookingCountAsync()
        {
            var result = await serviceTypeService.GetServiceTypesWithBookingCountAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }
    }
}