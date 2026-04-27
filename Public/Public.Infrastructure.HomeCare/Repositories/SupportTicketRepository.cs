using Infrastructure.HomeCare.Data;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Repositories;

public class SupportTicketRepository(HomeCareDbContext dbContext)
    : GenericRepository<SupportTicket>(dbContext), ISupportTicketRepository
{
}