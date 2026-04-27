using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Offer;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/offer")]
    [ApiController]
     [Authorize]

    public class OfferController(IOfferService offerService) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveOffersAsync()
        {
            var result = await offerService.GetActiveOffersAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("checkout-summary/{serviceId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCheckoutSummaryAsync(int serviceId)
        {
            var result = await offerService.GetCheckoutSummaryAsync(serviceId);
            return Ok(ResponseHelper.SuccessResponse(result));
        }


        [HttpPost("validate")]
        public async Task<IActionResult> ValidateCouponAsync(
            [FromBody] ValidateCouponRequestModel request)
        {
            var result = await offerService.ValidateCouponAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result, Messages.CouponAppliedSuccessfully));
        }
    }
}