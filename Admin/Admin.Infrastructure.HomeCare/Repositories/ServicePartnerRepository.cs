using Admin.Domain.HomeCare.DataModels.Request.ServicePartner;
using Admin.Domain.HomeCare.DataModels.Response.ServicePartner;
using Admin.Domain.HomeCare.Interface;
using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class ServicePartnerRepository(HomeCareDbContext dbContext)
        : GenericRepository<ServicePartner>(dbContext), IServicePartnerRepository
    {
        public async Task<ServicePartnerAttachment?> GetAttachmentAsync(
            int servicePartnerId, int attachmentId)
        {
            return await dbContext.ServicePartnerAttachments
                .FirstOrDefaultAsync(a =>
                    a.Id == attachmentId &&
                    a.ServicePartnerId == servicePartnerId &&
                    !a.IsDeleted);
        }

        public async Task<ServicePartnerDetailProjection> GetDetailProjectionAsync(int servicePartnerId)
        {
            var sp = await dbContext.ServicePartners
                .FirstOrDefaultAsync(x => x.Id == servicePartnerId)
                ?? throw new KeyNotFoundException($"Service partner {servicePartnerId} not found.");

            var skills = await dbContext.ServicePartnerSkills
                .Where(x => x.ServicePartnerId == servicePartnerId)
                .ToListAsync();

            var servicesOffered = await dbContext.ServicePartnerServicesOffered
                .Where(x => x.ServicePartnerId == servicePartnerId )
                .ToListAsync();

            var languages = await dbContext.ServicePartnerLanguages
                .Where(x => x.ServicePartnerId == servicePartnerId )
                .ToListAsync();

            var experiences = await dbContext.ServicePartnerExperiences
                .Where(x => x.ServicePartnerId == servicePartnerId )
                .ToListAsync();

            var attachments = await dbContext.ServicePartnerAttachments
                .Where(x => x.ServicePartnerId == servicePartnerId )
                .ToListAsync();

            var categoryIds = skills.Select(s => s.CategoryId).Distinct().ToList();
            var categoryNames = await dbContext.Categories
                .Where(c => categoryIds.Contains(c.Id) )
                .ToDictionaryAsync(c => c.Id, c => c.CategoryName);

            var subCategoryIds = servicesOffered.Select(o => o.SubCategoryId).Distinct().ToList();
            var subCategoryNames = await dbContext.SubCategories
                .Where(sc => subCategoryIds.Contains(sc.Id) )
                .ToDictionaryAsync(sc => sc.Id, sc => sc.SubCategoryName);

            var languageIds = languages.Select(l => l.LanguageId).Distinct().ToList();
            var languageNames = await dbContext.Languages
                .Where(l => languageIds.Contains(l.Id))
                .ToDictionaryAsync(l => l.Id, l => l.Name);

            var serviceTypeName = await dbContext.ServiceTypes
                .Where(st => st.Id == sp.ApplyingForTypeId )
               .Select(st => st.ServiceName)
                .FirstOrDefaultAsync() ?? string.Empty;

            return new ServicePartnerDetailProjection
            {
                ServicePartner = sp,
                Skills = skills,
                ServicesOffered = servicesOffered,
                Languages = languages,
                Experiences = experiences,
                Attachments = attachments,
                CategoryNames = categoryNames,
                SubCategoryNames = subCategoryNames,
                LanguageNames = languageNames,
                ServiceTypeName = serviceTypeName
            };
        }

        public async Task<IEnumerable<PartnerServiceResponseModel>> GetServicesByPartnerTypeAsync(
            int servicePartnerId)
        {
            var applyingForTypeId = await dbContext.ServicePartners
                .Where(sp => sp.Id == servicePartnerId && !sp.IsDeleted)
                .Select(sp => (int?)sp.ApplyingForTypeId)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Service partner {servicePartnerId} not found.");

            var services = await dbContext.Services
                .Include(s => s.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Where(s =>
                    !s.IsDeleted &&
                    s.IsAvailable &&
                    !s.SubCategory.IsDeleted &&
                    s.SubCategory.IsActive &&
                    !s.SubCategory.Category.IsDeleted &&
                    s.SubCategory.Category.ServiceTypeId == applyingForTypeId)
                .OrderBy(s => s.SubCategory.SubCategoryName)
                .ThenBy(s => s.Name)
                .Select(s => new PartnerServiceResponseModel
                {
                    ServiceId = s.Id,
                    ServiceName = s.Name,
                    SubCategoryName = s.SubCategory.SubCategoryName,
                    Duration = s.Duration,
                    Price = s.Price,
                    Commission = s.Commission,
                    IsAvailable = s.IsAvailable
                })
                .ToListAsync();

            return services;
        }

        public async Task<(int TotalRecords, IEnumerable<AssignedServiceRow> Rows)> GetAssignedBookingsAsync(
            int servicePartnerId,
            FilterAssignedServicesRequestModel filter)
        {
            var query = dbContext.Bookings
                .Include(b => b.User)
                .Include(b => b.Service)
                .Include(b => b.Address)
                .Where(b => b.AssignedPartnerId == servicePartnerId)
                .AsQueryable();

            if (filter.Date.HasValue)
                query = query.Where(b => b.BookingDate == filter.Date.Value);

            if (!string.IsNullOrWhiteSpace(filter.Time))
                query = query.Where(b => b.BookingTime == filter.Time);

            if (filter.ServiceStatus.HasValue)
                query = query.Where(b => b.Status == filter.ServiceStatus.Value);

            var totalRecords = await query.CountAsync();

            var bookings = (filter.PageNumber > 0 && filter.PageSize > 0)
                ? await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync()
                : await query.ToListAsync();

            var rows = bookings.Select(b =>
            {
                var bookingDateTime = b.BookingDate.ToDateTime(
                    TimeOnly.TryParse(b.BookingTime, out var t) ? t : TimeOnly.MinValue);

                return new AssignedServiceRow
                {
                    BookingId = b.Id,
                    ServiceId = b.ServiceId,
                    ServiceName = b.Service?.Name ?? string.Empty,
                    CustomerName = b.User?.Name ?? string.Empty,
                    DateAndTime = bookingDateTime.ToString("dd MMM yyyy h:mm tt"),
                    ServiceAddress = b.Address != null
                        ? $"{b.Address.HouseFlatNumber}, {b.Address.FullAddress}"
                        : string.Empty,
                    ServiceStatus = b.Status.ToString()
                };
            });

            return (totalRecords, rows);
        }

        public async Task<FilteredDataQueryResponse<GetServicePartnerResponseModel>> GetServicePartnersWithMetaAsync(
            FilterServicePartnerRequestModel filter)
        {
            var query =
                from sp in dbContext.ServicePartners
                select new
                {
                    sp.Id,
                    sp.FullName,
                    sp.MobileNumber,
                    sp.Email,
                    sp.ResidentialAddress,
                    sp.Status,
                    JobsCompleted = dbContext.Bookings.Count(
                        b => b.AssignedPartnerId == sp.Id && b.Status == Shared.HomeCare.Enums.BookingStatus.Completed),
                    JobTitle = sp.ServiceTypes != null
                        ? sp.ServiceTypes.ServiceName
                        : string.Empty
                };

            var maxJobsCompleted = await query.MaxAsync(x => (int?)x.JobsCompleted) ?? 0;
            if (!string.IsNullOrWhiteSpace(filter.ServiceTypeName))
                query = query.Where(x => x.JobTitle.ToLower().Contains(filter.ServiceTypeName.ToLower()));

            if (filter.JobsCompletedMin.HasValue)
                query = query.Where(x => x.JobsCompleted >= filter.JobsCompletedMin.Value);

            if (filter.JobsCompletedMax.HasValue)
                query = query.Where(x => x.JobsCompleted <= filter.JobsCompletedMax.Value);

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status.Value);

            bool isDesc = string.Equals(filter.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            query = filter.SortField?.ToLower() switch
            {
                "name" => isDesc ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName),
                "jobscompleted" => isDesc
                    ? query.OrderByDescending(x => x.JobsCompleted)
                    : query.OrderBy(x => x.JobsCompleted),
                _ => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
            };

            var totalRecords = await query.CountAsync();

            var records = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new GetServicePartnerResponseModel
                {
                    Id = x.Id,
                    Name = x.FullName,
                    MobileNumber = x.MobileNumber,
                    Email = x.Email,
                    Address = x.ResidentialAddress,
                    Job = x.JobTitle,
                    JobsCompleted = x.JobsCompleted,
                    Status = x.Status.ToString()
                })
                .ToListAsync();

            return new FilteredDataQueryResponse<GetServicePartnerResponseModel>
            {
                TotalRecords = totalRecords,
                Records = records,
                FilterMeta = new FilterRangeMeta
                {
                    MaxBookedServices = maxJobsCompleted
                }
            };

        }


        public async Task<int> GetPendingBookingCountAsync(int servicePartnerId)
        {
            return await dbContext.Bookings.CountAsync(b =>
                b.AssignedPartnerId == servicePartnerId &&
                !b.IsDeleted &&
                (b.Status == Shared.HomeCare.Enums.BookingStatus.Pending ||
                 b.Status == Shared.HomeCare.Enums.BookingStatus.Confirmed));
        }

        public async Task<int> GetOtherActivePartnerCountForServiceTypeAsync(int servicePartnerId)
        {
            var applyingForTypeId = await dbContext.ServicePartners
                .Where(sp => sp.Id == servicePartnerId && !sp.IsDeleted)
                .Select(sp => (int?)sp.ApplyingForTypeId)
                .FirstOrDefaultAsync();

            if (applyingForTypeId == null)
                return 0;

            return await dbContext.ServicePartners.CountAsync(sp =>
                sp.Id != servicePartnerId &&
                !sp.IsDeleted &&
                sp.ApplyingForTypeId == applyingForTypeId.Value &&
                sp.Status == Shared.HomeCare.Enums.ServicePartnerStatus.Active &&
                sp.VerificationStatus == Shared.HomeCare.Enums.VerificationStatus.Verified);
        }
    }
}