using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.AdminUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Admin.Api.HomeCare.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AdminUserController(IAdminUserService adminUserService) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<ActionResult> GetAllAsync([FromQuery] FilterAdminUserRequestModel filter)
        {
            var result = await adminUserService.GetAdminUsersAsync(filter);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetByIdAsync(int id)
        {
            var result = await adminUserService.GetAdminUserByIdAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateAdminUserRequestModel request)
        {
            if (request is null)
                return BadRequest(Messages.InvalidRequest);

            var result = await adminUserService.CreateAdminUserAsync(request);
            return Ok(ResponseHelper.CreateResponse(result,
                string.Format(Messages.CreatedSuccessfully, Messages.Admin)));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAsync(
            int id, [FromBody] UpdateAdminUserRequestModel request)
        {
            if (request is null || id != request.Id)
                return BadRequest(Messages.InvalidRequest);

            var result = await adminUserService.UpdateAdminUserAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.UpdatedSuccessfully, Messages.Admin)));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var result = await adminUserService.DeleteAdminUserAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.DeletedSuccessfully, Messages.Admin)));
        }

        [HttpPatch("change-password")]
        public async Task<ActionResult> ChangePasswordAsync(
            [FromBody] ChangeAdminUserPasswordRequestModel request)
        {
            if (request is null)
                return BadRequest(Messages.InvalidRequest);

            await adminUserService.ChangeAdminUserPasswordAsync(request);
            return Ok(ResponseHelper.SuccessResponse(null,
                string.Format(Messages.UpdatedSuccessfully, Messages.Password)));
        }
    }
}