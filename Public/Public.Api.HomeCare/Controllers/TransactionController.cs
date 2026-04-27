using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Payment;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/payment")]
    [ApiController]
    [Authorize]
    public class TransactionController(ITransactionService paymentService) : ControllerBase
    {
        [HttpPost("create-intent")]
        public async Task<IActionResult> CreatePaymentIntentAsync(
            [FromBody] CreateTransactionIntentRequest request)
        {
            var result = await paymentService.CreateTransactionIntentAsync(request);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPaymentAsync(
            [FromBody] ConfirmTransactionRequest request)
        {
            var result = await paymentService.ConfirmAndBookAsync(request);
            return Ok(ResponseHelper.CreateResponse(result,
                string.Format(Messages.CreatedSuccessfully, Messages.Booking)));
        }

        [HttpPost("failed")]
        public async Task<IActionResult> RecordFailedAsync(
            [FromBody] FailedTransactionRequest request)
        {
            await paymentService.RecordFailedTransactionAsync(request);
            return Ok(ResponseHelper.SuccessResponse(null!));
        }
    }
}