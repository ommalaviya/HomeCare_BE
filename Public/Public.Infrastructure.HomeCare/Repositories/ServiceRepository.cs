using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Public.Domain.HomeCare.Interface;
using Public.Domain.HomeCare.DataModels.Response.Home;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

public class ServiceRepository(HomeCareDbContext dbContext)
    : GenericRepository<ServicesOfSubCategory>(dbContext), IServiceRepository
{
    public async Task<List<ServiceWithBookingCount>> GetServicesWithImagesAsync()
    {
        var services = await dbContext.Services
            .Include(x => x.Images)
            .Include(x => x.SubCategory)
                .ThenInclude(sc => sc.Category)
            .Where(x => !x.IsDeleted)
            .ToListAsync();

        var bookingCounts = await dbContext.Bookings
            .GroupBy(b => b.ServiceId)
            .Select(g => new { ServiceId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ServiceId, x => x.Count);

        return services.Select(s => new ServiceWithBookingCount
        {
            Service = s,
            TotalBookings = bookingCounts.TryGetValue(s.Id, out var count) ? count : 0
        }).ToList();
    }
    
    public async Task<List<ServiceNamesResponseModel>> GetServiceNamesAsync()
    {
        return await dbContext.Services
            .Where(x => !x.IsDeleted)
            .Select(x => new ServiceNamesResponseModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .Distinct()
            .ToListAsync();
    }
}