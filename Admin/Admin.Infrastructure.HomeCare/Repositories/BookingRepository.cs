using Admin.Domain.HomeCare.DataModels.Request.Booking;
using Admin.Domain.HomeCare.DataModels.Response.Booking;
using Admin.Domain.HomeCare.Interface;
using AutoMapper;
using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Repositories;

namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class BookingRepository(HomeCareDbContext dbContext, IMapper mapper)
        : GenericRepository<Booking>(dbContext), IBookingRepository
    {
        private readonly HomeCareDbContext _db = dbContext;
        private readonly IMapper _mapper = mapper;

        // Parent grid
        public async Task<FilteredDataQueryResponse<CustomerBookingSummaryResponse>>
            GetCustomerBookingSummariesAsync(FilterBookingRequestModel filter)
        {
            var query = _db.Bookings
                .Include(b => b.User)
                .Include(b => b.Address)
                .Include(b => b.ServiceType)
                .Where(b => !b.IsDeleted && !b.User.IsDeleted)
                .AsQueryable();

            if (filter.ServiceTypeId.HasValue)
                query = query.Where(b => b.ServiceTypeId == filter.ServiceTypeId.Value);

            if (filter.Date.HasValue)
                query = query.Where(b => b.BookingDate == filter.Date.Value);

            if (!string.IsNullOrWhiteSpace(filter.Time))
                query = query.Where(b => b.BookingTime == filter.Time);

            if (filter.PaymentMethod.HasValue)
                query = query.Where(b => b.PaymentMethod == filter.PaymentMethod.Value);

            if (filter.Status.HasValue)
                query = query.Where(b => b.Status == filter.Status.Value);

            var groupedQuery = query
                .GroupBy(b => new
                {
                    b.UserId,
                    b.User.Name,
                    b.User.MobileNumber,
                    b.User.Email,
                    b.PaymentMethod
                })
                .Select(g => new
                {
                    g.Key.UserId,
                    CustomerName = g.Key.Name ?? string.Empty,
                    g.Key.MobileNumber,
                    g.Key.Email,
                    g.Key.PaymentMethod,
                    TotalBookedServices = g.Count(),
                    TotalBookingAmount = g.Sum(b => b.BookingAmount),
                    AddressId = g.OrderByDescending(b => b.Id).Select(b => b.AddressId).First()
                });

            var maxBookedServices = await groupedQuery.MaxAsync(x => (int?)x.TotalBookedServices) ?? 0;
            var maxBookingAmount = await groupedQuery.MaxAsync(x => (decimal?)x.TotalBookingAmount) ?? 0;

            if (filter.BookedServicesMin.HasValue)
                groupedQuery = groupedQuery.Where(g => g.TotalBookedServices >= filter.BookedServicesMin.Value);

            if (filter.BookedServicesMax.HasValue)
                groupedQuery = groupedQuery.Where(g => g.TotalBookedServices <= filter.BookedServicesMax.Value);

            if (filter.AmountMin.HasValue)
                groupedQuery = groupedQuery.Where(g => g.TotalBookingAmount >= filter.AmountMin.Value);

            if (filter.AmountMax.HasValue)
                groupedQuery = groupedQuery.Where(g => g.TotalBookingAmount <= filter.AmountMax.Value);

            var totalRecords = await groupedQuery.CountAsync();

            var pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
            var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;

            groupedQuery = (filter.SortField ?? string.Empty).ToLower() switch
            {
                "totalbookedservices" => filter.SortDirection == "desc"
                    ? groupedQuery.OrderByDescending(g => g.TotalBookedServices)
                    : groupedQuery.OrderBy(g => g.TotalBookedServices),

                "totalbookingamount" => filter.SortDirection == "desc"
                    ? groupedQuery.OrderByDescending(g => g.TotalBookingAmount)
                    : groupedQuery.OrderBy(g => g.TotalBookingAmount),

                _ => filter.SortDirection == "desc"
                    ? groupedQuery.OrderByDescending(g => g.CustomerName)
                    : groupedQuery.OrderBy(g => g.CustomerName)
            };

            var pagedItems = await groupedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var addressIds = pagedItems.Select(x => x.AddressId).Distinct().ToList();

            var addresses = await _db.UserAddresses
                .Where(a => addressIds.Contains(a.AddressId))
                .ToDictionaryAsync(a => a.AddressId, a => a.FullAddress ?? a.HouseFlatNumber);

            var records = pagedItems.Select(g => new CustomerBookingSummaryResponse
            {
                UserId = g.UserId,
                CustomerName = g.CustomerName,
                MobileNumber = g.MobileNumber,
                Email = g.Email,
                TotalBookedServices = g.TotalBookedServices,
                TotalBookingAmount = g.TotalBookingAmount,
                Address = addresses.TryGetValue(g.AddressId, out var addr) ? addr : string.Empty,
                PaymentMethod = g.PaymentMethod.ToString(),
            });

            return new FilteredDataQueryResponse<CustomerBookingSummaryResponse>
            {
                TotalRecords = totalRecords,
                Records = records,
                FilterMeta = new FilterRangeMeta
                {
                    MaxBookedServices = maxBookedServices,
                    MaxAmount = maxBookingAmount
                }
            };
        }

        // Child grid
        public async Task<IEnumerable<BookingDetailResponse>> GetBookingDetailsByUserIdAsync(
             int userId, FilterBookingRequestModel filter)
        {
            var query = _db.Bookings
                .Include(b => b.Service)
                .Include(b => b.ServiceType)
                .Include(b => b.AssignedPartner)
                .Where(b => b.UserId == userId
                         && b.PaymentMethod == filter.PaymentMethod
                         && !b.IsDeleted);

            if (filter.ServiceTypeId.HasValue)
                query = query.Where(b => b.ServiceTypeId == filter.ServiceTypeId.Value);

            if (filter.Date.HasValue)
                query = query.Where(b => b.BookingDate == filter.Date.Value);

            if (!string.IsNullOrWhiteSpace(filter.Time))
                query = query.Where(b => b.BookingTime == filter.Time);

            if (filter.Status.HasValue)
                query = query.Where(b => b.Status == filter.Status.Value);

            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookingDetailResponse>>(bookings);
        }

        // Customer Detail page
        public async Task<FilteredDataQueryResponse<CustomerBookingDetailResponse>> GetCustomerBookingsAsync(
            int customerId, FilterCustomerBookingsRequestModel filter)
        {
            var query = _db.Bookings
                .Include(b => b.Service)
                .Include(b => b.ServiceType)
                .Include(b => b.AssignedPartner)
                .Include(b => b.Address)
                .Where(b => b.UserId == customerId && !b.IsDeleted)
                .AsQueryable();

            if (filter.ServiceTypeId.HasValue)
                query = query.Where(b => b.ServiceTypeId == filter.ServiceTypeId.Value);

            if (filter.Date.HasValue)
                query = query.Where(b => b.BookingDate == filter.Date.Value);

            if (!string.IsNullOrWhiteSpace(filter.Time))
                query = query.Where(b => b.BookingTime == filter.Time);

            if (filter.AmountMin.HasValue)
                query = query.Where(b => b.BookingAmount >= filter.AmountMin.Value);

            if (filter.AmountMax.HasValue)
                query = query.Where(b => b.BookingAmount <= filter.AmountMax.Value);

            if (filter.PaymentMethod.HasValue)
                query = query.Where(b => b.PaymentMethod == filter.PaymentMethod.Value);

            if (filter.Status.HasValue)
                query = query.Where(b => b.Status == filter.Status.Value);

            var maxAmount = await query.MaxAsync(b => (decimal?)b.BookingAmount) ?? 0;

            bool isDesc = string.Equals(filter.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            query = (filter.SortField ?? string.Empty).ToLower() switch
            {
                "servicename" => isDesc ? query.OrderByDescending(b => b.Service.Name) : query.OrderBy(b => b.Service.Name),
                "servicetype" => isDesc ? query.OrderByDescending(b => b.ServiceType.ServiceName) : query.OrderBy(b => b.ServiceType.ServiceName),
                "bookingdate" => isDesc ? query.OrderByDescending(b => b.BookingDate) : query.OrderBy(b => b.BookingDate),
                "bookingamount" => isDesc ? query.OrderByDescending(b => b.BookingAmount) : query.OrderBy(b => b.BookingAmount),
                "status" => isDesc ? query.OrderByDescending(b => b.Status) : query.OrderBy(b => b.Status),
                _ => isDesc ? query.OrderByDescending(b => b.Id) : query.OrderBy(b => b.Id),
            };

            var totalRecords = await query.CountAsync();

            var pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
            var pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var records = _mapper.Map<IEnumerable<CustomerBookingDetailResponse>>(items);

            return new FilteredDataQueryResponse<CustomerBookingDetailResponse>
            {
                TotalRecords = totalRecords,
                Records = records,
                FilterMeta = new FilterRangeMeta
                {
                    MaxAmount = maxAmount
                }
            };
        }

        // Available experts
        public async Task<IEnumerable<AvailableExpertResponse>> GetAvailableExpertsAsync(
            int serviceTypeId, int? excludeBookingId = null)
        {
            DateOnly? targetDate = null;
            string? targetTime = null;
            int? currentAssignedPartnerId = null;

            if (excludeBookingId.HasValue)
            {
                var targetBooking = await _db.Bookings
                    .Where(b => b.Id == excludeBookingId.Value && !b.IsDeleted)
                    .Select(b => new { b.BookingDate, b.BookingTime, b.AssignedPartnerId })
                    .FirstOrDefaultAsync();

                if (targetBooking != null)
                {
                    targetDate = targetBooking.BookingDate;
                    targetTime = targetBooking.BookingTime;
                    currentAssignedPartnerId = targetBooking.AssignedPartnerId;
                }
            }

            var busyPartnerIds = await _db.Bookings
                .Where(b => !b.IsDeleted
                         && b.AssignedPartnerId.HasValue
                         && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
                         && (!excludeBookingId.HasValue || b.Id != excludeBookingId.Value)
                         && (targetDate == null || b.BookingDate == targetDate.Value)
                         && (targetTime == null || b.BookingTime == targetTime))
                .Select(b => b.AssignedPartnerId!.Value)
                .Distinct()
                .ToListAsync();

            var partners = await _db.ServicePartners
                .Where(p =>
                    !p.IsDeleted &&
                    p.ApplyingForTypeId == serviceTypeId &&
                    p.Status == ServicePartnerStatus.Active &&
                    p.VerificationStatus == VerificationStatus.Verified &&
                    !busyPartnerIds.Contains(p.Id) &&
                    (!currentAssignedPartnerId.HasValue || p.Id != currentAssignedPartnerId.Value))
                .OrderBy(p => p.FullName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AvailableExpertResponse>>(partners);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
            => await _db.Users.FindAsync(userId);

        public async Task<int?> GetAssignedPartnerIdAsync(int bookingId)
        {
            return await _db.Bookings
                .Where(b => b.Id == bookingId && !b.IsDeleted)
                .Select(b => b.AssignedPartnerId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAndPaymentAsync(
            int userId, PaymentMethod paymentMethod)
        {
            return await _db.Bookings
                .Include(b => b.Transaction)
                .Where(b => b.UserId == userId
                         && b.PaymentMethod == paymentMethod
                         && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task IncrementPartnerTotalJobsCompletedAsync(int partnerId)
        {
            var partner = await _db.ServicePartners.FindAsync(partnerId);
            if (partner != null)
            {
                partner.TotalJobsCompleted += 1;
                _db.ServicePartners.Update(partner);
            }
        }
        public async Task<bool> HasActiveBookingsForServiceAsync(int serviceId)
        {
            return await _db.Bookings.AnyAsync(b =>
                b.ServiceId == serviceId &&
                !b.IsDeleted &&
                (b.Status == BookingStatus.Pending));
        }

        public async Task<bool> HasActiveBookingsForSubCategoryAsync(int subCategoryId)
        {
            return await _db.Bookings.AnyAsync(b =>
                !b.IsDeleted &&
                (b.Status == BookingStatus.Pending) &&
                _db.Services.Any(s => s.Id == b.ServiceId && s.SubCategoryId == subCategoryId && !s.IsDeleted));
        }

        public async Task<bool> HasActiveBookingsForCategoryAsync(int categoryId)
        {
            return await _db.Bookings.AnyAsync(b =>
                !b.IsDeleted &&
                (b.Status == BookingStatus.Pending) &&
                _db.Services.Any(s =>
                    s.Id == b.ServiceId &&
                    !s.IsDeleted &&
                    _db.SubCategories.Any(sc => sc.Id == s.SubCategoryId && sc.CategoryId == categoryId && !sc.IsDeleted)));
        }

        public async Task<bool> HasActiveBookingsForServiceTypeAsync(int serviceTypeId)
        {
            return await _db.Bookings.AnyAsync(b =>
                b.ServiceTypeId == serviceTypeId &&
                !b.IsDeleted &&
                (b.Status == BookingStatus.Pending));
        }
    }
}