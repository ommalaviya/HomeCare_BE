using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Public.Infrastructure.HomeCare.Repositories
{
    public class TransactionRepository(HomeCareDbContext dbContext)
        : GenericRepository<Transaction>(dbContext), ITransactionRepository
    {
        public async Task<Transaction?> GetByStripeIntentIdAsync(string intentId)
            => await dbContext.Transactions
                .Include(t => t.Booking)
                .FirstOrDefaultAsync(t => t.StripePaymentIntentId == intentId);

        public async Task<Transaction?> GetByBookingIdAsync(int bookingId)
            => await dbContext.Transactions
                .FirstOrDefaultAsync(t => t.BookingId == bookingId);
    }
}