using System;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Admin.Domain.HomeCare.Interface
{
    public interface ISupportTicketRepository : IGenericRepository<SupportTicket>
    {
    }
}