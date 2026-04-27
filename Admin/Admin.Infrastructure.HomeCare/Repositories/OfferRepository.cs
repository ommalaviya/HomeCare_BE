using Infrastructure.HomeCare.Data;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;
using Admin.Domain.HomeCare.Interface;
using Admin.Domain.HomeCare.DataModels.Response.Offer;
using Shared.HomeCare.DataModel.Response;
using Admin.Domain.HomeCare.DataModels.Request.Offer;
using Microsoft.EntityFrameworkCore;

namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class OfferRepository(HomeCareDbContext dbContext) : GenericRepository<Offer>(dbContext), IOfferRepository
    {
        public async Task<FilteredDataQueryResponse<GetOfferResponseModel>> GetOffersWithMetaAsync(
                   FilterOfferRequestModel filter)
        {
            var query = dbContext.Offers
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            var maxAppliedCount = await query.MaxAsync(x => (int?)x.AppliedCount) ?? 0;

            if (filter.DiscountPercentage.HasValue)
                query = query.Where(x => x.DiscountPercentage == filter.DiscountPercentage.Value);

            if (filter.AppliedCountMin.HasValue)
                query = query.Where(x => x.AppliedCount >= filter.AppliedCountMin.Value);

            if (filter.AppliedCountMax.HasValue)
                query = query.Where(x => x.AppliedCount <= filter.AppliedCountMax.Value);

            if (filter.Availability.HasValue)
                query = query.Where(x => x.IsActive == filter.Availability.Value);

            bool isDesc = string.Equals(filter.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            query = filter.SortField?.ToLower() switch
            {
                "discountpercentage" => isDesc
                    ? query.OrderByDescending(x => x.DiscountPercentage)
                    : query.OrderBy(x => x.DiscountPercentage),
                "appliedcount" => isDesc
                    ? query.OrderByDescending(x => x.AppliedCount)
                    : query.OrderBy(x => x.AppliedCount),
                _ => isDesc
                    ? query.OrderByDescending(x => x.Id)
                    : query.OrderBy(x => x.Id),
            };

            var totalRecords = await query.CountAsync();

            var records = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new GetOfferResponseModel
                {
                    Id = x.Id,
                    CouponCode = x.CouponCode,
                    CouponDescription = x.CouponDescription,
                    DiscountPercentage = x.DiscountPercentage,
                    AppliedCount = x.AppliedCount,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return new FilteredDataQueryResponse<GetOfferResponseModel>
            {
                TotalRecords = totalRecords,
                Records = records,
                FilterMeta = new FilterRangeMeta
                {
                    MaxBookingCount = maxAppliedCount
                }
            };
        }
    }
}