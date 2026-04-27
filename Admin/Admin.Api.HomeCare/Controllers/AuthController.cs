using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Auth;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.Resources;
using System.Security.Claims;

namespace Admin.Api.HomeCare.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            var result = await authService.LoginAsync(model);
            return Ok(ResponseHelper.SuccessResponse(result, Messages.SuccessResponse));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var result = await authService.RefreshTokenAsync();
            return Ok(ResponseHelper.SuccessResponse(result, Messages.TokenRefreshedSuccessfully));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? User.FindFirstValue("sub");

            if (int.TryParse(adminIdClaim, out var adminId))
                await authService.RevokeSessionAsync(adminId);

            return Ok(ResponseHelper.SuccessResponse(null, Messages.LoggedOut));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
        {
            var message = await authService.ForgotPasswordAsync(model);
            return Ok(ResponseHelper.SuccessResponse(null, message));
        }

        [HttpGet("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken([FromQuery] string token)
        {
            await authService.ValidateResetTokenAsync(token);
            return Ok(ResponseHelper.SuccessResponse(null, Messages.ValidLink));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
        {
            var message = await authService.ResetPasswordAsync(model);
            return Ok(ResponseHelper.SuccessResponse(null, message));
        }
    }
}