using System.Linq.Expressions;
using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Interfaces.Repositories;

namespace Shared.Interfaces.Services
{
    public interface IGenericService<TEntity> where TEntity : class
    {
        int CurrentUserId { get; }

        Task<TEntity?> GetByIdAsync(int id);

        /// <summary>Fetches by id and throws KeyNotFoundException if null or soft-deleted.</summary>
        Task<TEntity> GetOrThrowAsync(int id, string notFoundMessage);

        Task<DataQueryResponse<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            PageRequest? model = null,
            ExpressionIncluder<TEntity>? includer = null);

        TResponse ToResponseModel<TResponse>(dynamic obj) where TResponse : class;

        TDest Map<TDest, TSource>(TSource source) where TDest : class;

        TDest Map<TDest, TSource>(TSource source, TDest destination) where TDest : class;

        Task<bool> CheckDuplicateAsync(Expression<Func<TEntity, bool>> predicate);

        Task ThrowIfDuplicateAsync(
            Expression<Func<TEntity, bool>> predicate,
            string duplicateMessage);

        Task<TEntity> FindOrThrowAsync(
             Expression<Func<TEntity, bool>> predicate,
                     string notFoundMessage,
             ExpressionIncluder<TEntity>? includer = null);

        TEntity ToEntity(dynamic obj);

        Task<TEntity> AddAsync(TEntity entity);

        Task AddRangeForAsync(IEnumerable<TEntity> entities);

        Task UpdateAsync(TEntity entity);

        Task SoftDeleteAsync(TEntity entity);
        Task AddRangeAsync<TChildEntity>(IGenericRepository<TChildEntity> repository, IEnumerable<TChildEntity> entities) where TChildEntity : BaseEntity;
        Task RemoveRangeAsync<TChildEntity>(IGenericRepository<TChildEntity> repository, IEnumerable<TChildEntity> entities) where TChildEntity : class;
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    }
}
