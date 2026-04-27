using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Public.Domain.HomeCare.Interface
{
    public interface IAddressRepository : IGenericRepository<UserAddress>
    {
        Task<IEnumerable<UserAddress>> GetByUserIdAsync(int userId);
    }
}