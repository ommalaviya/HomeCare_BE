using Admin.Domain.HomeCare.DataModels.Request.Transaction;
using Admin.Domain.HomeCare.DataModels.Response.Transaction;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.DataModel.Request;
using Shared.Interfaces.Services;

namespace Admin.Application.HomeCare.Interfaces
{
    public interface ITransactionService : IGenericService<Transaction>
    {
        Task<FilteredDataQueryResponseModel<GetTransactionResponseModel>> GetAllTransactionsAsync(
            FilterTransactionRequestModel? filter = null);

        Task<TransactionDetailResponseModel?> GetTransactionDetailAsync(int id);

        Task<FilteredDataQueryResponseModel<GetUserTransactionResponseModel>> GetTransactionsByUserIdAsync(int userId, PageRequest pageRequest);

        Task<bool> softDelteTransactionAsync(int id);
    }
}
