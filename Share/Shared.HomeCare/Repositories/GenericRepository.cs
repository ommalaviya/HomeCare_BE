using Microsoft.EntityFrameworkCore;
using Shared.HomeCare.Interfaces.Repositories;
using System.Linq.Expressions;
using Infrastructure.HomeCare.Data;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.DataModel.Request;
 
namespace Shared.HomeCare.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly HomeCareDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
 
        public GenericRepository(HomeCareDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
 
        public async Task<TEntity?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);
 
        public virtual async Task<DataQueryResponse<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate,
            PageRequest? model = null,
            ExpressionIncluder<TEntity>? includer = null)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
 
            if (includer?.Includes != null)
            {
                foreach (var include in includer.Includes)
                    query = query.Include(include);
            }
 
            if (includer?.ThenIncludes != null)
            {
                foreach (var thenInclude in includer.ThenIncludes)
                    query = thenInclude(query);
            }
 
            if (predicate != null)
                query = query.Where(predicate);

            if (!string.IsNullOrWhiteSpace(model?.SortField))
            {
                    var propertyInfo = typeof(TEntity).GetProperty(model.SortField, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (propertyInfo != null)
                    {
                        var param = Expression.Parameter(typeof(TEntity), "x");
                        var property = Expression.Property(param, propertyInfo);
                        var lambda = Expression.Lambda(property, param);
                        var methodName = string.Equals(model.SortDirection, "desc", StringComparison.OrdinalIgnoreCase)
                            ? "OrderByDescending"
                            : "OrderBy";
                        var orderByMethod = typeof(Queryable)
                            .GetMethods()
                            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                            .MakeGenericMethod(typeof(TEntity), property.Type);
                        query = (IQueryable<TEntity>)orderByMethod.Invoke(null, new object[] { query, lambda })!;
                    }
            }

            return new DataQueryResponse<TEntity>
            {
                TotalRecords = await query.CountAsync(),
                Records = (model?.PageNumber > 0 && model.PageSize > 0)
                    ? await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync()
                    : await query.ToListAsync(),
            };
        }
 
        public async Task AddAsync(TEntity entity)
            => await _dbSet.AddAsync(entity);
 
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
            => await _dbSet.AddRangeAsync(entities);
 
        public Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
 
        public Task RemoveAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
 
        public Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }
 
        public Task<bool> CheckDuplicateAsync(Expression<Func<TEntity, bool>> predicate)
            => _dbSet.AnyAsync(predicate);
 
        public async Task<TEntity?> FindDataAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            ExpressionIncluder<TEntity>? includer = null)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
 
            if (includer?.Includes != null)
            {
                foreach (var include in includer.Includes)
                    query = query.Include(include);
            }
            if (includer?.ThenIncludes != null)
                foreach (var thenInclude in includer.ThenIncludes)
                    query = thenInclude(query);
 
            if (predicate != null)
                query = query.Where(predicate);
 
            return await query.FirstOrDefaultAsync();
        }
 
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return predicate == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);
        }

        public async Task<decimal> MaxAsync(Expression<Func<TEntity, decimal>> selector)
        {
            // Guard: MaxAsync throws on an empty set, so return 0 when no rows exist.
            var hasAny = await _dbSet.AnyAsync();
            if (!hasAny) return 0;
            return await _dbSet.MaxAsync(selector);
        }
    }
}