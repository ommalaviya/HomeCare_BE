using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ServiceDetailController(IServiceDetailService serviceDetailService) : ControllerBase
    {
        [HttpGet("detail/{id:int}")]
        public async Task<IActionResult> GetServiceDetailAsync([FromRoute] int id)
        {
           
            var result = await serviceDetailService.GetServiceDetailAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result));
        }
    }
}