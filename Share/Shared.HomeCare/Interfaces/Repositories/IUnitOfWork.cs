namespace Shared.HomeCare.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        int Save();
        Task SaveChangesAsync();
    }
}
