using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Booking;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/booking")]
    [ApiController]
    [Authorize]
    public class BookingController(IBookingService bookingService, IReceiptService receiptService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateBookingRequestModel request)
        {
            var result = await bookingService.CreateBookingAsync(request);
            return Ok(ResponseHelper.CreateResponse(result,
                string.Format(Messages.CreatedSuccessfully, Messages.Booking)));
        }

        [HttpGet("my-booking")]
        public async Task<IActionResult> GetMyBookingsAsync()
        {
            var result = await bookingService.GetMyBookingsAsync();
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookingsByTabAsync(
            [FromQuery] MyBookingsRequestModel request)
        {
            var result = await bookingService.GetMyBookingsByTabAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("slot-availability")]
        public async Task<IActionResult> CheckSlotAvailabilityAsync(
            [FromQuery] SlotAvailabilityRequestModel request)
        {
            var result = await bookingService.CheckSlotAvailabilityAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

         [HttpGet("{bookingId:int}/invoice")]
        public async Task<IActionResult> DownloadInvoiceAsync(int bookingId)
        {
            var pdfBytes = await receiptService.GenerateBookingInvoiceAsync(bookingId);
 
            return File(
                pdfBytes,
                "application/pdf",
                $"Invoice_Booking_{bookingId}.pdf");
        }
    }
}