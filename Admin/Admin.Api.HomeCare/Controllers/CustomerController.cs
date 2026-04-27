using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Booking;
using Admin.Domain.HomeCare.DataModels.Request.Customer;
using Admin.Domain.HomeCare.DataModels.Response.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Admin.Api.HomeCare.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CustomerController(ICustomerService customerService, IBookingService bookingService) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<ActionResult> GetAllAsync([FromQuery] FilterCustomerRequestModel filter)
        {
            var result = await customerService.GetAllCustomersAsync(filter);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDetailResponse>> GetByIdAsync(int id)
        {
            var result = await customerService.GetCustomerDetailAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("{id:int}/bookings")]
        public async Task<ActionResult> GetCustomerBookingsAsync(
        int id,
        [FromQuery] FilterCustomerBookingsRequestModel filter)
        {
            var result = await bookingService.GetCustomerBookingsAsync(id, filter);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<ActionResult<GetCustomerResponseModel>> CreateAsync(
            [FromBody] CreateCustomerRequestModel request)
        {
            if (request is null)
                return BadRequest(Messages.InvalidRequest);

            var result = await customerService.CreateCustomerAsync(request);
            return Ok(ResponseHelper.CreateResponse(result, string.Format(Messages.CreatedSuccessfully, Messages.User)));
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<bool>> UpdateStatusAsync(int id)
        {
            var result = await customerService.UpdateStatusAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result, string.Format(Messages.UpdatedSuccessfully, Messages.UserStatus)));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            var result = await customerService.SoftDeleteCustomerAsync(id);
            return Ok(ResponseHelper.SuccessResponse(result, string.Format(Messages.DeletedSuccessfully, Messages.User)));
        }
    }
}