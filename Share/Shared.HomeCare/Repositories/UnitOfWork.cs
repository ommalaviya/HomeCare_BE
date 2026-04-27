using Infrastructure.HomeCare.Data;

using Shared.HomeCare.Interfaces.Repositories;

namespace Shared.HomeCare.Repositories
{
    public class UnitOfWork(HomeCareDbContext context) : IUnitOfWork
    {
        public int Save() => context.SaveChanges();

        public void Dispose() => context.Dispose();

        public async Task SaveChangesAsync() => await context.SaveChangesAsync();
    }
}