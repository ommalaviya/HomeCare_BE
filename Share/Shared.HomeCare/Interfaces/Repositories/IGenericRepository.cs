using System.Linq.Expressions;
using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.DataModel.Response;

namespace Shared.HomeCare.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(int id);

        Task<DataQueryResponse<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            PageRequest? pageRequest = null,
            ExpressionIncluder<TEntity>? includer = null);

        Task AddAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entities);

        Task UpdateAsync(TEntity entity);

        Task RemoveAsync(TEntity entity);

        Task<bool> CheckDuplicateAsync(Expression<Func<TEntity, bool>> predicate);

        Task RemoveRangeAsync(IEnumerable<TEntity> entities);
        
        Task<TEntity?> FindDataAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            ExpressionIncluder<TEntity>? includer = null);

        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
         Task<decimal> MaxAsync(Expression<Func<TEntity, decimal>> selector);

    }
}
