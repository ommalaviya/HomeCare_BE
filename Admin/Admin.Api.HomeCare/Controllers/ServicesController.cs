using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Services;
using Admin.Domain.HomeCare.DataModels.Response.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Admin.Api.HomeCare.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ServicesController(IServicesService servicesService) : ControllerBase
    {
        [HttpPost("get")]
        public async Task<IActionResult> GetBySubCategoryAsync([FromBody] FilterServicesRequestModel request)
        {
            var result = await servicesService.GetServicesBySubCategoryAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("get/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await servicesService.GetServiceByIdAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("by-service-type/{serviceTypeId:int}")]
        public async Task<IActionResult> GetByServiceTypeAsync(int serviceTypeId)
        {
            var result = await servicesService.GetAllByServiceTypeAsync(serviceTypeId);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpPost("add")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<GetServiceByIdResponseModel>> CreateAsync(
            [FromForm] CreateServiceRequestModel request)
        {
            if (request is null)
                return BadRequest(Messages.InvalidRequest);

            var result = await servicesService.CreateServiceAsync(request);

            return Ok(ResponseHelper.CreateResponse(result, string.Format(Messages.CreatedSuccessfully, Messages.Services)));
        }

        [HttpPut("update/{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<GetServiceByIdResponseModel>> UpdateAsync(
            int id, [FromForm] UpdateServiceRequestModel request)
        {
            if (request is null || id != request.Id)
                return BadRequest(Messages.InvalidRequest);

            var result = await servicesService.UpdateServiceAsync(request);

            return Ok(ResponseHelper.SuccessResponse(result, string.Format(Messages.UpdatedSuccessfully, Messages.Services)));
        }

        [HttpPatch("{id:int}/availability")]
        public async Task<IActionResult> ToggleAvailabilityAsync(int id, [FromBody] bool isAvailable)
        {
            var result = await servicesService.ToggleAvailabilityAsync(id, isAvailable);
            return Ok(ResponseHelper.SuccessResponse(result, string.Format(Messages.UpdatedSuccessfully, Messages.Services)));
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await servicesService.DeleteServiceAsync(id);

            return Ok(ResponseHelper.SuccessResponse(result, string.Format(Messages.DeletedSuccessfully, Messages.Services)));
        }
    }
}