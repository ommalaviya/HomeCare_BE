using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.Resources;

namespace Admin.Api.HomeCare.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    [Authorize]
    public class TransactionController(ITransactionService transactionService) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<IActionResult> GetAllAsync([FromQuery] FilterTransactionRequestModel filter)
        {
            var result = await transactionService.GetAllTransactionsAsync(filter);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetailAsync([FromRoute] int id)
        {
            var result = await transactionService.GetTransactionDetailAsync(id);
            if (result is null)
                return Ok(ResponseHelper.FailedResponse(result));

            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUserAsync([FromRoute] int userId, [FromQuery] PageRequest pageRequest)
        {
            var result = await transactionService.GetTransactionsByUserIdAsync(userId, pageRequest);
            return Ok(ResponseHelper.SuccessResponse(result));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await transactionService.softDelteTransactionAsync(id);

            return Ok(
                ResponseHelper.SuccessResponse(
                    result,
                    string.Format(Messages.DeletedSuccessfully, Messages.Transaction)
                )
            );
        }
    }
}
