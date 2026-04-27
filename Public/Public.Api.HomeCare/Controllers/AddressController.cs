using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Address;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/address")]
    [ApiController]
    [Authorize]
    public class AddressController(IAddressService addressService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetMyAddressesAsync()
        {
            var result = await addressService.GetMyAddressesAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("{addressId:int}")]
        public async Task<IActionResult> GetByIdAsync(int addressId)
        {
            var result = await addressService.GetAddressByIdAsync(addressId);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateAddressRequestModel request)
        {
            var result = await addressService.CreateAddressAsync(request);
            return Ok(ResponseHelper.CreateResponse(result,
                string.Format(Messages.CreatedSuccessfully, Messages.Address)));
        }

        [HttpPut("{addressId:int}")]
        public async Task<IActionResult> UpdateAsync(int addressId,
            [FromBody] UpdateAddressRequestModel request)
        {
            var result = await addressService.UpdateAddressAsync(addressId, request);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.UpdatedSuccessfully, Messages.Address)));
        }

        [HttpDelete("{addressId:int}")]
        public async Task<IActionResult> DeleteAsync(int addressId)
        {
            var deleted = await addressService.DeleteAddressAsync(addressId);
            if (!deleted)
                return NotFound(ResponseHelper.FailedResponse(null,
                    string.Format(Messages.NotFound, Messages.Address)));

            return Ok(ResponseHelper.SuccessResponse(null,
                string.Format(Messages.DeletedSuccessfully, Messages.Address)));
        }

        [HttpPost("reverse-geocode")]
        [AllowAnonymous]
        public async Task<IActionResult> ReverseGeocodeAsync(
            [FromBody] ReverseGeocodeRequestModel request)
        {
            var result = await addressService.ReverseGeocodeAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchAsync([FromQuery] string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery) || searchQuery.Trim().Length < 3)
                return Ok(ResponseHelper.SuccessResponse(new List<object>()));

            var results = await addressService.SearchAddressAsync(searchQuery.Trim());
            return Ok(ResponseHelper.SuccessResponse(results));
        }

    }
}