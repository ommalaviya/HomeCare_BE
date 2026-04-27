using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Resources;

namespace Admin.API.HomeCare.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        // Parent grid 
        [HttpGet("list")]
        public async Task<ActionResult> GetCustomerBookingSummariesAsync(
            [FromQuery] FilterBookingRequestModel filter)
        {
            var result = await bookingService.GetCustomerBookingSummariesAsync(filter);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        // Child grid 
        [HttpGet("{userId:int}/details")]
        public async Task<ActionResult> GetBookingDetailsByUserIdAsync(
            int userId,
            [FromQuery] FilterBookingRequestModel filter)
        {
            var result = await bookingService.GetBookingDetailsByUserIdAsync(userId, filter);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        // Available experts 
        [HttpGet("available-experts")]
        public async Task<ActionResult> GetAvailableExpertsAsync(
            [FromQuery] int serviceTypeId,
            [FromQuery] int? excludeBookingId = null)
        {
            var result = await bookingService.GetAvailableExpertsAsync(serviceTypeId, excludeBookingId);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        // Change expert 
        [HttpPut("change-expert")]
        public async Task<ActionResult> ChangeExpertAsync([FromBody] ChangeExpertRequestModel request)
        {
            if (request is null)
                return BadRequest(Messages.InvalidRequest);

            var result = await bookingService.ChangeExpertAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.UpdatedSuccessfully, Messages.ServicePartner)));
        }

        //  Complete booking 
        [HttpPut("{bookingId:int}/complete")]
        public async Task<ActionResult> CompleteBookingAsync(int bookingId)
        {
            var result = await bookingService.CompleteBookingAsync(bookingId);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.UpdatedSuccessfully, Messages.Booking)));
        }

        // Cancel booking 
        [HttpPut("cancel")]
        public async Task<ActionResult> CancelBookingAsync([FromBody] CancelBookingRequestModel request)
        {
            if (request is null)
                return BadRequest(Messages.InvalidRequest);

            var result = await bookingService.CancelBookingAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.UpdatedSuccessfully, Messages.Booking)));
        }

        //  Delete by payment (parent grid) 
        [HttpDelete("customer/{userId:int}/payment/{paymentMethod}")]
        public async Task<ActionResult> DeleteBookingsByPaymentAsync(
             int userId,
             PaymentMethod paymentMethod)
        {
            var result = await bookingService.DeleteBookingsByPaymentAsync(userId, paymentMethod);
            return Ok(ResponseHelper.SuccessResponse(result, string.Format(Messages.DeletedSuccessfully, Messages.Booking)));
        }

        // Delete booking (child grid) 
        [HttpDelete("{bookingId:int}")]
        public async Task<ActionResult> DeleteBookingAsync(int bookingId)
        {
            var result = await bookingService.DeleteBookingAsync(bookingId);
            return Ok(ResponseHelper.SuccessResponse(result,
                string.Format(Messages.DeletedSuccessfully, Messages.Booking)));
        }
    }
}