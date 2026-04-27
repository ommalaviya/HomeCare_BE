using Microsoft.AspNetCore.Mvc;
using Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Admin;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.HomeCare.Interfaces.Services;
using Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Shared.HomeCare.Resources;

[ApiController]
[Route("api/admin/profile")]
[Authorize]
public class AdminProfileController : ControllerBase
{
    private readonly IAdminProfileService _service;
    private readonly IFileService _fileService;

    public AdminProfileController(IAdminProfileService service, IFileService fileService)
    {
        _service = service;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _service.GetProfileAsync();
        return Ok(ResponseHelper.SuccessResponse(result));
    }

    [HttpPatch("contact")]
    public async Task<IActionResult> UpdateContact([FromBody] UpdateAdminContactRequest model)
    {
        await _service.UpdateContactAsync(model);
        return Ok(ResponseHelper.SuccessResponse(null,
            string.Format(Messages.UpdatedSuccessfully, Messages.Contact)));
    }

    [HttpPatch("password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdateAdminPasswordRequest model)
    {
        await _service.UpdatePasswordAsync(model);
        return Ok(ResponseHelper.SuccessResponse(null, string.Format(Messages.UpdatedSuccessfully, Messages.Password)));
    }

    [HttpPatch("image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateImage([FromForm] UpdateProfileImageRequest request)
    {
        if (request.Image == null || request.Image.Length == 0)
            return BadRequest(ResponseHelper.FailedResponse(null, Messages.FileRequired));

        await _service.UpdateProfileImageAsync(request.Image);
        return Ok(ResponseHelper.SuccessResponse(null, string.Format(Messages.UpdatedSuccessfully, Messages.Image)));
    }

    [HttpGet("image/{imageName}")]
    public async Task<FileContentHttpResult> GetProfileImage(string imageName)
    {
        return await _fileService.GetImageResultAsync(imageName, "admins");
    }
}