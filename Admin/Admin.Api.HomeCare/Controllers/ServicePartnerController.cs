using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.ServicePartner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Admin.Api.HomeCare.Controllers
{
    [Route("api/service-partners")]
    [Authorize]
    [ApiController]
    public class ServicePartnerController(IServicePartnerService servicePartnerService) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<IActionResult> GetAllAsync([FromQuery] FilterServicePartnerRequestModel filter)
        {
            var result = await servicePartnerService.GetAllServicePartnersAsync(filter);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetailAsync(int id)
        {
            var result = await servicePartnerService.GetDetailAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("{id:int}/services")]
        public async Task<IActionResult> GetPartnerServicesAsync(int id)
        {
            var result = await servicePartnerService.GetPartnerServicesAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpPatch("{id:int}/approve")]
        public async Task<IActionResult> ApproveAsync(int id)
        {
            var result = await servicePartnerService.ApproveAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result, result.Message));
        }

        [HttpPatch("{id:int}/reject")]
        public async Task<IActionResult> RejectAsync(int id, [FromBody] RejectServicePartnerRequestModel request)
        {
            var result = await servicePartnerService.RejectAsync(id, request);
            return Ok(ResponseHelper.SuccessResponse(result, result.Message));
        }

        [HttpGet("{id:int}/assigned-services")]
        public async Task<IActionResult> GetAssignedServicesAsync(
            int id,
            [FromQuery] FilterAssignedServicesRequestModel filter)
        {
            var result = await servicePartnerService.GetAssignedServicesAsync(id, filter);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpPatch("{id:int}/toggle-status")]
        public async Task<IActionResult> ToggleStatusAsync(int id)
        {
            var result = await servicePartnerService.ToggleStatusAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.UpdatedSuccessfully, Messages.ServicePartner)));
        }

        [HttpDelete("{id:int}/delete")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await servicePartnerService.DeleteServicePartnerAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.DeletedSuccessfully, Messages.ServicePartner)));
        }

        [HttpGet("{id:int}/attachments/{attachmentId:int}/download")]
        public async Task<IResult> DownloadAttachmentAsync(int id, int attachmentId)
        {
            return await servicePartnerService.DownloadAttachmentAsync(id, attachmentId);
        }
    }
}