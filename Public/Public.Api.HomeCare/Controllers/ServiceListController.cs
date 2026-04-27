using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/service-list")]
    [AllowAnonymous]
    [ApiController]
    public class ServiceListController(IServiceListService serviceListService) : ControllerBase
    {
        [HttpGet("service-type")]
        public async Task<IActionResult> GetCategoriesWithSubCategoriesAsync(int serviceTypeId)
        {
            var result = await serviceListService
                .GetCategoriesWithSubCategoriesByServiceTypeIdAsync(serviceTypeId);

            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("subcategory")]
        public async Task<IActionResult> GetServicesBySubCategoryAsync(int subCategoryId)
        {
            var result = await serviceListService
                .GetServicesBySubCategoryIdAsync(subCategoryId);

            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("services")]
        public async Task<IActionResult> SearchServicesAsync(
            [FromQuery] int serviceTypeId,
            [FromQuery] string? term = null)
        {
            var result = await serviceListService
                .SearchServicesAsync(serviceTypeId, term);
                
            return Ok(ResponseHelper.SuccessResponse(result));
        }
    }
}
