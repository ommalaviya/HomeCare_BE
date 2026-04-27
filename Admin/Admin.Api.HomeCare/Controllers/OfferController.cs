using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Offer;
using Admin.Domain.HomeCare.DataModels.Response.Offer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Admin.API.HomeCare.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OfferController(IOfferService offerService) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<ActionResult> GetAllAsync([FromQuery] FilterOfferRequestModel filter)
        {
            var result = await offerService.GetOffersAsync(filter);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetByIdAsync(int id)
        {
            var result = await offerService.GetOfferByIdAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<ActionResult<GetOfferResponseModel>> CreateAsync(
            [FromBody] CreateOfferRequestModel request)
        {
            if (request is null)
                return BadRequest(Messages.InvalidRequest);

            var result = await offerService.CreateOfferAsync(request);
            return Ok(ResponseHelper.CreateResponse(result, string.Format(Messages.CreatedSuccessfully, Messages.Offer)));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<GetOfferResponseModel>> UpdateAsync(
            int id, [FromBody] UpdateOfferRequestModel request)
        {
            if (request is null || id != request.Id)
                return BadRequest(Messages.InvalidRequest);

            var result = await offerService.UpdateOfferAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result, string.Format(Messages.UpdatedSuccessfully, Messages.Offer)));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            var result = await offerService.SoftDeleteOfferAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result, string.Format(Messages.DeletedSuccessfully, Messages.Offer)));
        }
    }
}