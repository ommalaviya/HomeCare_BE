using Infrastructure.HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using Public.Domain.HomeCare.DataModels.Response.ServiceType;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

public class ServiceTypeRepository(HomeCareDbContext dbContext)
    : GenericRepository<ServiceTypes>(dbContext), IServiceTypeRepository
{
    public async Task<List<ServiceTypeWithBookingCountDto>> GetServiceTypesWithBookingCountAsync()
    {
        return await dbContext.ServiceTypes
            .Where(st => !st.IsDeleted)
            .Select(st => new ServiceTypeWithBookingCountDto
            {
                Id = st.Id,
                ServiceName = st.ServiceName,
                ImageName = st.ImageName,
                TotalBookings = dbContext.Bookings.Count(b => b.ServiceTypeId == st.Id && !b.IsDeleted)
            })
            .ToListAsync();
    }
}