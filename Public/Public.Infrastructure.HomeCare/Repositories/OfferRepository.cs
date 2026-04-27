using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Public.Infrastructure.HomeCare.Repositories
{
    public class OfferRepository(HomeCareDbContext dbContext)
        : GenericRepository<Offer>(dbContext), IOfferRepository
    {
        public async Task<bool> HasUserUsedCouponAsync(int userId, int offerId)
        {
            return await dbContext.CouponUsages
                .AnyAsync(cu => cu.UserId == userId && cu.CouponId == offerId);
        }

        public async Task AddCouponUsageAsync(CouponUsage couponUsage)
        {
            await dbContext.CouponUsages.AddAsync(couponUsage);
        }
 
        public async Task IncrementAppliedCountAsync(int offerId)
        {
            var offer = await dbContext.Offers.FindAsync(offerId);
            if (offer != null)
            {
                offer.AppliedCount += 1;
                dbContext.Offers.Update(offer);
            }
        }
    }
}