using Public.Domain.HomeCare.DataModels.Request.Payment;
using Public.Domain.HomeCare.DataModels.Response.Payment;
using Public.Domain.HomeCare.DataModels.Response.Booking;

namespace Public.Application.HomeCare.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionIntentResponse> CreateTransactionIntentAsync(CreateTransactionIntentRequest request);
        Task<BookingResponseModel> ConfirmAndBookAsync(ConfirmTransactionRequest request);
        Task RecordFailedTransactionAsync(FailedTransactionRequest request);
    }
}