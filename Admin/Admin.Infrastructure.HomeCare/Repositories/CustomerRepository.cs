using Admin.Domain.HomeCare.DataModels.Request.Customer;
using Admin.Domain.HomeCare.DataModels.Response.Customer;
using Admin.Domain.HomeCare.Interface;
using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Repositories;

namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class CustomerRepository(HomeCareDbContext dbContext)
        : GenericRepository<User>(dbContext), ICustomerRepository
    {
        public async Task<FilteredDataQueryResponse<GetCustomerResponseModel>> GetCustomersWithBookingCountAsync(
            FilterCustomerRequestModel filter)
        {
            var query =
            from u in dbContext.Users
            where !u.IsDeleted
            let totalBookings = dbContext.Bookings.Count(b => b.UserId == u.Id
                && !b.IsDeleted
                && b.Status != BookingStatus.Pending
                && b.Status != BookingStatus.Cancelled)
            let pendingBookings = dbContext.Bookings.Count(b => b.UserId == u.Id
                && !b.IsDeleted
                && b.Status == BookingStatus.Pending)
            select new
            {
                u.Id,
                u.Name,
                u.MobileNumber,
                u.Email,
                u.Status,
                TotalBookings = totalBookings,
                PendingBookings = pendingBookings
            };

            var maxBookingCount = await query.MaxAsync(x => (int?)x.TotalBookings) ?? 0;

            if (!string.IsNullOrEmpty(filter.Status))
            {
                if (filter.Status == "Active")
                    query = query.Where(x => x.Status == "Active");
                else if (filter.Status == "Blocked")
                    query = query.Where(x => x.Status == "Block");
            }

            if (filter.BookingMin.HasValue)
                query = query.Where(x => x.TotalBookings >= filter.BookingMin.Value);

            if (filter.BookingMax.HasValue)
                query = query.Where(x => x.TotalBookings <= filter.BookingMax.Value);

            bool isDesc = string.Equals(filter.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            query = filter.SortField?.ToLower() switch
            {
                "name" => isDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "totalbookings" => isDesc ? query.OrderByDescending(x => x.TotalBookings) : query.OrderBy(x => x.TotalBookings),
                _ => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
            };

            var totalRecords = await query.CountAsync();

            var records = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new GetCustomerResponseModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    MobileNumber = x.MobileNumber,
                    Email = x.Email,
                    Status = x.Status,
                    TotalBookings = x.TotalBookings,
                    PendingBookings = x.PendingBookings
                })
                .ToListAsync();

            return new FilteredDataQueryResponse<GetCustomerResponseModel>
            {
                TotalRecords = totalRecords,
                Records = records,
                FilterMeta = new FilterRangeMeta
                {
                    MaxBookingCount = maxBookingCount
                }
            };
        }

        public async Task<CustomerDetailResponse?> GetCustomerDetailAsync(int customerId)
        {
            return await dbContext.Users
                .Where(u => u.Id == customerId && !u.IsDeleted)
                .Select(u => new CustomerDetailResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    MobileNumber = u.MobileNumber,
                    Email = u.Email,
                    Status = u.Status
                })
                .FirstOrDefaultAsync();
        }

        public async Task CancelActiveBookingsByUserIdAsync(int userId, string cancellationReason)
        {
            var activeBookings = await dbContext.Bookings
                .Where(b => b.UserId == userId
                         && !b.IsDeleted
                         && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed))
                .ToListAsync();

            foreach (var booking in activeBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.AssignedPartnerId = null;
                booking.CancellationReason = cancellationReason;
            }

            if (activeBookings.Count > 0)
                await dbContext.SaveChangesAsync();
        }
    }
}