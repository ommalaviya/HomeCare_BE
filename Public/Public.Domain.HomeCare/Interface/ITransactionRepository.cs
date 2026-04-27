using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Public.Domain.HomeCare.Interface
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<Transaction?> GetByStripeIntentIdAsync(string intentId);
        Task<Transaction?> GetByBookingIdAsync(int bookingId);
    }
}