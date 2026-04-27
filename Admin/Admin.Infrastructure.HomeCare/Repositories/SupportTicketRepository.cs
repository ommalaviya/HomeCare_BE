using Infrastructure.HomeCare.Data;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;
using Admin.Domain.HomeCare.Interface;
namespace Admin.Infrastructure.HomeCare.Repositories
{
    public class SupportTicketRepository(HomeCareDbContext dbContext) : GenericRepository<SupportTicket>(dbContext), ISupportTicketRepository
    {
    }
}