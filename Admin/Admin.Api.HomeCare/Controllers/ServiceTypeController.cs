using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.ServiceTypes;
using Admin.Domain.HomeCare.DataModels.Response.ServiceTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Admin.Api.HomeCare.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class ServiceTypeController(IServiceTypeService serviceTypeService) : ControllerBase
    {
        [HttpGet("get")]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await serviceTypeService.GetServiceTypesAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("get/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await serviceTypeService.GetServiceTypeByIdAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("{id:int}/image")]
        public async Task<IResult> GetImageAsync(int id)
        {
            // Returns FileContentHttpResult
            return await serviceTypeService.GetServiceTypeImageAsync(id);
        }

        [HttpPost("add")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<GetServiceTypeResponseModel>> CreateAsync(
            [FromForm] CreateServiceTypeRequestModel request)
        {
            if (request == null)
                return BadRequest(Messages.InvalidRequest);

            var result = await serviceTypeService.CreateServiceTypeAsync(request);

            return Ok(ResponseHelper.CreateResponse(result, string.Format(Messages.CreatedSuccessfully, Messages.ServiceType)));
        }

        [HttpPut("update/{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<GetServiceTypeResponseModel>> UpdateAsync(
            int id, [FromForm] UpdateServiceTypeRequestModel request)
        {
            if (request is null || id != request.Id)
                return BadRequest(Messages.InvalidRequest);

            var result = await serviceTypeService.UpdateServiceTypeAsync(request);

            return Ok(ResponseHelper.SuccessResponse(result, string.Format(Messages.UpdatedSuccessfully, Messages.ServiceType)));
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await serviceTypeService.SoftDeleteServiceTypeAsync(id);

            return Ok(ResponseHelper.SuccessResponse(result, string.Format(Messages.DeletedSuccessfully, Messages.ServiceType)));
        }
    }
}