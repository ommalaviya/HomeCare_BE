using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.ServicePartners;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/service-partner")]
    [ApiController]
    public class ServicePartnerController(IServicePartnerService servicePartnerService) : ControllerBase
    {
        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ResponseHelper.FailedResponse(null, Messages.FileRequired));

            var imageName = await servicePartnerService.UploadProfileImageAsync(file);
            return Ok(ResponseHelper.SuccessResponse(new { imageName },
                string.Format(Messages.AddedSuccessfully, "Profile Image")));
        }

        [HttpGet("profile-image/{id}")]
        public async Task<FileContentHttpResult> GetProfileImageAsync(string id)
        {
            return await servicePartnerService.GetProfileImageAsync(id);
        }

        [HttpPost("upload-attachment")]
        public async Task<IActionResult> UploadAttachmentAsync(IFormFile file, [FromForm] string? documentLabel)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ResponseHelper.FailedResponse(null, Messages.FileRequired));

            var result = await servicePartnerService.UploadAttachmentAsync(file, documentLabel);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.AddedSuccessfully, Messages.File)));
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyAsync([FromBody] ApplyServicePartnerRequestModel request)
        {
            if (request == null)
                return BadRequest(ResponseHelper.FailedResponse(null, Messages.InvalidRequest));

            var result = await servicePartnerService.ApplyAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.CreatedSuccessfully, Messages.ServicePartner)));
        }
    }
}
