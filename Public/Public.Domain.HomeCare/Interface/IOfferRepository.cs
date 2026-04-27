using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Public.Domain.HomeCare.Interface
{
    public interface IOfferRepository : IGenericRepository<Offer>
    {
        Task<bool> HasUserUsedCouponAsync(int userId, int offerId);

        Task AddCouponUsageAsync(CouponUsage couponUsage);
        
        Task IncrementAppliedCountAsync(int offerId);
    }
}