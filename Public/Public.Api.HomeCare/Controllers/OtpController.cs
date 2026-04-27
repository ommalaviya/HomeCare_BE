using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Users;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/otp")]
    [ApiController]
    public class OtpController(IOtpService otpService) : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> SendOtpAsync([FromBody] SendOtpRequestModel request)
        {
            await otpService.SendOtpAsync(request.Email);
            return Ok(ResponseHelper.SuccessResponse(null, Messages.OtpSentSuccessfully));
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtpAsync([FromBody] VerifyOtpRequestModel request)
        {
            var result = await otpService.VerifyOtpAsync(request.Email, request.Otp, request.Name);

            if (result == null)
                return Unauthorized(ResponseHelper.FailedResponse(null, Messages.InvalidOrExpiredOtp));

            return Ok(ResponseHelper.SuccessResponse(result, Messages.OtpVerifiedSuccessfully));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var result = await otpService.RefreshTokenAsync();

            if (result == null)
                return Unauthorized(ResponseHelper.FailedResponse(null, Messages.InvalidOrExpiredRefreshToken));

            return Ok(ResponseHelper.SuccessResponse(result, Messages.TokenRefreshedSuccessfully));
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            otpService.Logout();
            return Ok(ResponseHelper.SuccessResponse(null, Messages.LoggedOut));
        }
    }
}
