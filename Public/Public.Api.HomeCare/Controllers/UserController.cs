using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Users;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await userService.GetUsersAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfileAsync()
        {
            var result = await userService.GetProfileAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateOrUpdateAsync(
            [FromBody] CreateOrUpdateUserRequestModel request)
        {
            var result = await userService.CreateOrUpdateUserAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.CreatedSuccessfully, Messages.User)));
        }

        [HttpPost("profile/email/send-otp")]
        public async Task<IActionResult> SendEmailUpdateOtpAsync(
            [FromBody] SendEmailUpdateOtpRequestModel request)
        {
            await userService.SendEmailUpdateOtpAsync(request.NewEmail);
            return Ok(ResponseHelper.SuccessResponse(null, Messages.OtpSentSuccessfully));
        }

        [HttpPut("profile/email")]
        public async Task<IActionResult> UpdateEmailAsync(
            [FromBody] UpdateEmailRequestModel request)
        {
            var result = await userService.UpdateEmailAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.UpdatedSuccessfully, Messages.EmailId)));
        }

        [HttpPut("profile/phone")]
        public async Task<IActionResult> UpdatePhoneAsync(
            [FromBody] UpdatePhoneRequestModel request)
        {
            var result = await userService.UpdatePhoneAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.UpdatedSuccessfully, Messages.MobileNumber)));
        }

    }
}